using Dalamud.Configuration;
using System;

namespace HouseKeeper;

[Serializable]
public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 0;

    public bool IsConfigWindowMovable { get; set; } = true;
    public bool EnableNotifications { get; set; } = true;
    public bool NotifyOnHouseEntry { get; set; } = true;
    public int ReminderIntervalDays { get; set; } = 45;
    public DateTime LastHouseEntryUtc { get; set; } = DateTime.MinValue;
    public DateTime LastReminderUtc { get; set; } = DateTime.MinValue;

    // The below exists just to make saving less cumbersome
    public void Save()
    {
        Plugin.PluginInterface.SavePluginConfig(this);
    }
}
