using System;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Windowing;

namespace HouseKeeper.Windows;

public class ConfigWindow : Window, IDisposable
{
    private readonly Configuration configuration;

    // We give this window a constant ID using ###.
    // This allows for labels to be dynamic, like "{FPS Counter}fps###XYZ counter window",
    // and the window ID will always be "###XYZ counter window" for ImGui
    public ConfigWindow(Plugin plugin) : base("HouseKeeper Settings###With a constant ID")
    {
        Flags = ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
                ImGuiWindowFlags.NoScrollWithMouse;

        Size = new Vector2(360, 240);
        SizeCondition = ImGuiCond.Always;

        configuration = plugin.Configuration;
    }

    public void Dispose() { }

    public override void PreDraw()
    {
        // Flags must be added or removed before Draw() is being called, or they won't apply
        if (configuration.IsConfigWindowMovable)
        {
            Flags &= ~ImGuiWindowFlags.NoMove;
        }
        else
        {
            Flags |= ImGuiWindowFlags.NoMove;
        }
    }

    public override void Draw()
    {
        ImGui.Text("HouseKeeper settings");
        ImGui.Separator();

        var lastEntry = configuration.LastHouseEntryUtc;
        var lastEntryDisplay = TimeDisplay.FormatUtcDate(lastEntry);
        ImGui.Text($"Last entry: {lastEntryDisplay}");

        var expiryDisplay = "Pending first entry";
        if (lastEntry != DateTime.MinValue)
        {
            var dueDate = lastEntry.AddDays(configuration.ReminderIntervalDays);
            var remaining = dueDate - DateTime.UtcNow;
            expiryDisplay = remaining <= TimeSpan.Zero
                ? $"Expired {TimeDisplay.FormatTimeSpan(remaining)} ago"
                : TimeDisplay.FormatTimeSpan(remaining);
        }

        ImGui.Text($"Time to expiry: {expiryDisplay}");
        ImGui.Spacing();

        var enableNotifications = configuration.EnableNotifications;
        if (ImGui.Checkbox("Enable reminders", ref enableNotifications))
        {
            configuration.EnableNotifications = enableNotifications;
            configuration.Save();
        }

        var notifyOnEntry = configuration.NotifyOnHouseEntry;
        if (ImGui.Checkbox("Notify on house entry", ref notifyOnEntry))
        {
            configuration.NotifyOnHouseEntry = notifyOnEntry;
            configuration.Save();
        }

        var reminderDays = configuration.ReminderIntervalDays;
        if (ImGui.InputInt("Reminder interval (days)", ref reminderDays))
        {
            configuration.ReminderIntervalDays = Math.Max(1, reminderDays);
            configuration.Save();
        }

        var alertBeforeExpiry = configuration.AlertBeforeExpiryDays;
        if (ImGui.InputInt("Alert before expiry (days)", ref alertBeforeExpiry))
        {
            configuration.AlertBeforeExpiryDays = Math.Max(0, alertBeforeExpiry);
            configuration.Save();
        }

        var movable = configuration.IsConfigWindowMovable;
        if (ImGui.Checkbox("Movable Config Window", ref movable))
        {
            configuration.IsConfigWindowMovable = movable;
            configuration.Save();
        }
    }
}
