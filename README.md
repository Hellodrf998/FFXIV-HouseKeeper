# HouseKeeper

HouseKeeper is a Dalamud plugin that reminds you to enter your personal estate hall every 45 days to avoid housing demolition. It tracks the last time you entered an estate hall, notifies you when the timer is due, and provides a simple UI to review and adjust the reminder interval.

## Features

- Automatic tracking when you enter an estate hall.
- Chat reminders once the configured interval has elapsed (default: 45 days).
- Status window with last entry time, next reminder due, and manual reset.
- Settings window to toggle notifications and adjust the reminder interval.

## Slash Command

- `/housekeeper` — Open the HouseKeeper status window.

## Prerequisites

- XIVLauncher, FINAL FANTASY XIV, and Dalamud installed and run at least once.
- .NET 9 SDK (or whichever version your IDE installs for you).
- Optional: set `DALAMUD_HOME` if your Dalamud dev path is not the default.

## Building

1. Open `HouseKeeper.sln` in Visual Studio 2022 or JetBrains Rider.
2. Build the solution (Debug or Release).
3. The output DLL is located at `HouseKeeper/bin/x64/<Configuration>/HouseKeeper.dll`.

## Installing as a Dev Plugin

1. In game, open Dalamud settings with `/xlsettings` and add the full path to `HouseKeeper.dll` under **Experimental → Dev Plugin Locations**.
2. Open the Plugin Installer (`/xlplugins`), then **Dev Tools → Installed Dev Plugins** and enable **HouseKeeper**.

## Configuration

Open `/housekeeper` and click **Show Settings** to:

- Enable or disable reminders.
- Toggle the notification when an entry is recorded.
- Change the reminder interval (in days).

## Notes

HouseKeeper records entries based on entering estate halls. If you want to reset the timer manually, use the **Mark Entry Now** button in the status window.
