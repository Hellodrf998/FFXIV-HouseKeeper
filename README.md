# HouseKeeper

HouseKeeper is a Dalamud plugin that tracks your last personal estate entry and reminds you before the 45-day demolition timer expires.

## Features

- Records the last time you enter a personal estate hall.
- Reminds you before the timer expires based on your alert lead time.
- Shows last entry time and time to expiry in the settings UI.
- Allows manual updates via the status window.

## Commands

- `/housekeeper` — Open the status window.

## Settings

Open the settings window from the status window or the Dalamud plugin settings UI.

- **Enable reminders**: Toggle reminder notifications.
- **Notify on house entry**: Show a chat message when a house entry is recorded.
- **Reminder interval (days)**: Default 45 days.
- **Alert before expiry (days)**: How many days before expiry to start reminders.
- **Last entry / Time to expiry**: Informational fields.

## Building

1. Open `HouseKeeper.sln` in your C# editor (Visual Studio 2022 or JetBrains Rider).
2. Build the solution (Debug or Release).
3. The resulting plugin can be found at `HouseKeeper/bin/x64/Debug/HouseKeeper.dll` (or `Release` if appropriate).

## Activating in-game

1. Launch the game and use `/xlsettings` in chat or `xlsettings` in the Dalamud Console to open Dalamud settings.
    - Go to `Experimental`, and add the full path to the `HouseKeeper.dll` to the list of Dev Plugin Locations.
2. Use `/xlplugins` (chat) or `xlplugins` (console) to open the Plugin Installer.
    - Go to `Dev Tools > Installed Dev Plugins`, and the `HouseKeeper` entry should be visible. Enable it.

Dalamud loads `HouseKeeper/HouseKeeper.json` next to your DLL for metadata in the Plugin Installer.
