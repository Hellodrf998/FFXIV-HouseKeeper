using System;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Windowing;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;

namespace HouseKeeper.Windows;

public class MainWindow : Window, IDisposable
{
    private readonly Plugin plugin;

    // We give this window a hidden ID using ##.
    // The user will see "My Amazing Window" as window title,
    // but for ImGui the ID is "My Amazing Window##With a hidden ID"
    public MainWindow(Plugin plugin)
        : base("HouseKeeper Status##With a hidden ID", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 240),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        this.plugin = plugin;
    }

    public void Dispose() { }

    public override void Draw()
    {
        ImGui.Text("House entry reminders for personal estates.");
        ImGui.Text($"Reminders enabled: {(plugin.Configuration.EnableNotifications ? "Yes" : "No")}");
        ImGui.Text($"Reminder interval: {plugin.Configuration.ReminderIntervalDays} days");
        ImGui.Text($"Alert before expiry: {plugin.Configuration.AlertBeforeExpiryDays} days");

        var lastEntry = plugin.Configuration.LastHouseEntryUtc;
        var lastEntryDisplay = TimeDisplay.FormatUtcDate(lastEntry);
        ImGui.Text($"Last recorded entry: {lastEntryDisplay}");

        var nextDue = plugin.Configuration.LastHouseEntryUtc == DateTime.MinValue
            ? "Pending first entry"
            : plugin.Configuration.LastHouseEntryUtc.AddDays(plugin.Configuration.ReminderIntervalDays)
                .ToLocalTime()
                .ToString("f");
        ImGui.Text($"Next reminder due: {nextDue}");

        var timeToExpiry = "Pending first entry";
        if (lastEntry != DateTime.MinValue)
        {
            var dueDate = lastEntry.AddDays(plugin.Configuration.ReminderIntervalDays);
            var remaining = dueDate - DateTime.UtcNow;
            timeToExpiry = remaining <= TimeSpan.Zero
                ? $"Expired {TimeDisplay.FormatTimeSpan(remaining)} ago"
                : TimeDisplay.FormatTimeSpan(remaining);
        }

        ImGui.Text($"Time to expiry: {timeToExpiry}");

        ImGuiHelpers.ScaledDummy(8.0f);

        if (ImGui.Button("Show Settings"))
        {
            plugin.ToggleConfigUi();
        }

        ImGui.SameLine();
        if (ImGui.Button("Mark Entry Now"))
        {
            plugin.Configuration.LastHouseEntryUtc = DateTime.UtcNow;
            plugin.Configuration.LastReminderUtc = DateTime.MinValue;
            plugin.Configuration.Save();
        }

        ImGuiHelpers.ScaledDummy(8.0f);

        using (var child = ImRaii.Child("HouseKeeperHelp", Vector2.Zero, true))
        {
            if (child.Success)
            {
                ImGui.TextWrapped($"Enter your estate hall at least once every {plugin.Configuration.ReminderIntervalDays} days to avoid housing demolition. HouseKeeper records the last time you entered a personal estate hall and reminds you when it's time to refresh that timer.");
            }
        }
    }
}
