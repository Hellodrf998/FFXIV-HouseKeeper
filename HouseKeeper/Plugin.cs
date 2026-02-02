using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using System;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using HouseKeeper.Windows;
using Lumina.Excel.Sheets;

namespace HouseKeeper;

public sealed class Plugin : IDalamudPlugin
{
    [PluginService] internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] internal static ICommandManager CommandManager { get; private set; } = null!;
    [PluginService] internal static IClientState ClientState { get; private set; } = null!;
    [PluginService] internal static IDataManager DataManager { get; private set; } = null!;
    [PluginService] internal static IFramework Framework { get; private set; } = null!;
    [PluginService] internal static IChatGui ChatGui { get; private set; } = null!;
    [PluginService] internal static IPluginLog Log { get; private set; } = null!;

    private const string CommandName = "/housekeeper";
    private static readonly TimeSpan ReminderCheckInterval = TimeSpan.FromMinutes(1);
    private static readonly TimeSpan ReminderCooldown = TimeSpan.FromHours(24);

    public Configuration Configuration { get; init; }

    public readonly WindowSystem WindowSystem = new("HouseKeeper");
    private ConfigWindow ConfigWindow { get; init; }
    private MainWindow MainWindow { get; init; }
    private DateTime lastReminderCheckUtc = DateTime.MinValue;

    public Plugin()
    {
        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();

        ConfigWindow = new ConfigWindow(this);
        MainWindow = new MainWindow(this);

        WindowSystem.AddWindow(ConfigWindow);
        WindowSystem.AddWindow(MainWindow);

        CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
        {
            HelpMessage = "Open the HouseKeeper status window."
        });

        // Tell the UI system that we want our windows to be drawn through the window system
        PluginInterface.UiBuilder.Draw += WindowSystem.Draw;

        // This adds a button to the plugin installer entry of this plugin which allows
        // toggling the display status of the configuration ui
        PluginInterface.UiBuilder.OpenConfigUi += ToggleConfigUi;

        // Adds another button doing the same but for the main ui of the plugin
        PluginInterface.UiBuilder.OpenMainUi += ToggleMainUi;

        ClientState.TerritoryChanged += OnTerritoryChanged;
        Framework.Update += OnFrameworkUpdate;

        if (Configuration.LastHouseEntryUtc == DateTime.MinValue)
        {
            Configuration.LastHouseEntryUtc = DateTime.UtcNow;
            Configuration.Save();
        }

        Log.Information($"HouseKeeper initialized for {PluginInterface.Manifest.Name}.");
    }

    public void Dispose()
    {
        // Unregister all actions to not leak anything during disposal of plugin
        PluginInterface.UiBuilder.Draw -= WindowSystem.Draw;
        PluginInterface.UiBuilder.OpenConfigUi -= ToggleConfigUi;
        PluginInterface.UiBuilder.OpenMainUi -= ToggleMainUi;
        ClientState.TerritoryChanged -= OnTerritoryChanged;
        Framework.Update -= OnFrameworkUpdate;

        WindowSystem.RemoveAllWindows();

        ConfigWindow.Dispose();
        MainWindow.Dispose();

        CommandManager.RemoveHandler(CommandName);
    }

    private void OnCommand(string command, string args)
    {
        // In response to the slash command, toggle the display status of our main ui
        MainWindow.Toggle();
    }

    private void OnFrameworkUpdate(IFramework framework)
    {
        if (!ClientState.IsLoggedIn || !Configuration.EnableNotifications)
        {
            return;
        }

        var now = DateTime.UtcNow;
        if (now - lastReminderCheckUtc < ReminderCheckInterval)
        {
            return;
        }

        lastReminderCheckUtc = now;

        if (Configuration.LastHouseEntryUtc == DateTime.MinValue)
        {
            Configuration.LastHouseEntryUtc = now;
            Configuration.Save();
            return;
        }

        var dueDate = Configuration.LastHouseEntryUtc.AddDays(Configuration.ReminderIntervalDays);
        if (now < dueDate)
        {
            return;
        }

        if (now - Configuration.LastReminderUtc < ReminderCooldown)
        {
            return;
        }

        SendChatNotification($"It has been {Configuration.ReminderIntervalDays} days since your last recorded house entry. Visit your estate hall to refresh the timer.");
        Configuration.LastReminderUtc = now;
        Configuration.Save();
    }

    private void OnTerritoryChanged(ushort territoryId)
    {
        if (!ClientState.IsLoggedIn)
        {
            return;
        }

        if (!IsHouseInterior((uint)territoryId))
        {
            return;
        }

        Configuration.LastHouseEntryUtc = DateTime.UtcNow;
        Configuration.LastReminderUtc = DateTime.MinValue;
        Configuration.Save();

        if (Configuration.NotifyOnHouseEntry)
        {
            SendChatNotification($"House entry recorded. The {Configuration.ReminderIntervalDays}-day timer has been reset.");
        }
    }

    private bool IsHouseInterior(uint territoryId)
    {
        var sheet = DataManager.GetExcelSheet<TerritoryType>();
        if (sheet == null || !sheet.TryGetRow(territoryId, out var territoryRow))
        {
            return false;
        }

        var placeName = territoryRow.PlaceName.Value.Name.ToString();
        return placeName.Contains("Estate Hall", StringComparison.OrdinalIgnoreCase);
    }

    private void SendChatNotification(string message)
    {
        ChatGui.Print($"[HouseKeeper] {message}");
    }
    
    public void ToggleConfigUi() => ConfigWindow.Toggle();
    public void ToggleMainUi() => MainWindow.Toggle();
}
