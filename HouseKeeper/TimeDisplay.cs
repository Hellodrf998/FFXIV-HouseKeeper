using System;
using System.Collections.Generic;
using System.Globalization;

namespace HouseKeeper;

public static class TimeDisplay
{
    public static string FormatUtcDate(DateTime utcTimestamp)
    {
        if (utcTimestamp == DateTime.MinValue)
        {
            return "Not recorded yet";
        }

        return utcTimestamp.ToLocalTime().ToString("f", CultureInfo.CurrentCulture);
    }

    public static string FormatTimeSpan(TimeSpan span)
    {
        var duration = span.Duration();
        if (duration < TimeSpan.FromMinutes(1))
        {
            return "less than a minute";
        }

        var parts = new List<string>();
        if (duration.Days > 0)
        {
            parts.Add($"{duration.Days} day{(duration.Days == 1 ? string.Empty : "s")}");
        }

        if (duration.Hours > 0)
        {
            parts.Add($"{duration.Hours} hour{(duration.Hours == 1 ? string.Empty : "s")}");
        }

        if (duration.Days == 0 && duration.Minutes > 0)
        {
            parts.Add($"{duration.Minutes} minute{(duration.Minutes == 1 ? string.Empty : "s")}");
        }

        return string.Join(" ", parts);
    }
}
