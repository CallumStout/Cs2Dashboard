# CS2 Dashboard

CS2 Dashboard is a desktop app that listens to Counter-Strike 2 Game State Integration (GSI) events on `http://localhost:3000/` and shows live match stats.

It is built with .NET 8 + Avalonia and uses the `CounterStrike2GSI` library.

## What It Does

- Starts a local GSI listener on port `3000`
- Tries to auto-generate a CS2 GSI config named `cs2dashboard`
- Reads live player stats from incoming game state payloads
- Displays per-player:
  - Name
  - Kills
  - Deaths
  - Assists
  - Last updated time
- Displays current map
- Supports light/dark mode toggle in the UI

## How It Works

1. App startup:
- `App` creates `MainWindow` and assigns `Program.MainViewModel`
- `Program.StartGsiListener()` starts `GameStateListener` on port `3000`

2. GSI data flow:
- CS2 sends JSON game state updates to `http://localhost:3000/`
- `Program.OnNewGameState(...)` handles each update
- Player stats are pulled from `AllPlayers` (or fallback `Player`)
- Stats are stored in `StatsService` (in-memory dictionary)
- `StatsService` raises `StatsChanged`
- `MainWindowViewModel` updates UI collections on Avalonia UI thread

3. UI updates:
- `Views/MainWindow.axaml` binds to `MainWindowViewModel`
- Player list, map, and status labels update live

## Requirements

- Windows (for included installer flow)
- .NET SDK 8.0+ (for local build/run)
- Counter-Strike 2 installed and running
- CS2 Game State Integration enabled/configured

## Run Locally (Developer)

From the repository root:

```powershell
cd cs2dashboard
dotnet restore
dotnet run
```

The app should start and show:
- Endpoint: `Listening for CS2 GSI on http://localhost:3000/`
- GSI config status message

## Publish (Standalone Build)

```powershell
cd cs2dashboard
dotnet publish -c Release -r win-x64 --self-contained true
```

Publish output is expected under:

`cs2dashboard/bin/Release/net8.0/win-x64/publish/`

## Build Installer (Inno Setup)

Installer script:

`Installer/Setup.iss`

Expected binary input folder in script:

`..\cs2dashboard\bin\Release\net8.0\win-x64\publish`

Installer output folder in script:

`..\Release\97Solutions`

Current setup metadata in script:
- App name: `CS2 Stats Dashboard`
- Version: `1.0.0.1`
- EXE: `Cs2Dashboard.exe`

## CS2 GSI Config

The app tries to generate the GSI config automatically on startup.

If auto-generation fails, create a config manually in your CS2 `cfg` directory, for example:

```cfg
"CS2 Dashboard"
{
  "uri" "http://127.0.0.1:3000"
  "timeout" "5.0"
  "buffer" "0.1"
  "throttle" "0.1"
  "heartbeat" "30.0"
  "data"
  {
    "provider" "1"
    "map" "1"
    "player_id" "1"
    "player_state" "1"
    "player_match_stats" "1"
    "allplayers_id" "1"
    "allplayers_state" "1"
    "allplayers_match_stats" "1"
  }
}
```

Save as something like:

`gamestate_integration_cs2dashboard.cfg`

Then restart CS2.

## What This App Is Not

- Not a web dashboard or cloud service
- Not a persistent stats tracker (no database/history)
- Not VAC/anti-cheat bypass tooling
- Not a replacement for HLTV/Faceit analytics
- Not intended for internet-exposed endpoints (listener is local)

## Current Limitations

- Data is memory-only and resets when app closes
- Listener endpoint/port is hardcoded (`3000`)
- No filtering by team, match, or map history
- No export (CSV/JSON) or replay timeline
- No automated tests in this repository currently

## Project Structure

- `cs2dashboard/Program.cs`: app entry + GSI listener lifecycle
- `cs2dashboard/StatsService.cs`: in-memory stats store + change events
- `cs2dashboard/ViewModels/MainWindowViewModel.cs`: UI state + bindings
- `cs2dashboard/Views/MainWindow.axaml`: main dashboard UI
- `Installer/Setup.iss`: Windows installer definition

## Troubleshooting

- App shows no player updates:
  - Ensure CS2 is running and in a match/session with GSI updates
  - Verify CS2 GSI config exists and points to `http://127.0.0.1:3000`
  - Check local firewall rules for loopback/port `3000`
- "Could not auto-create GSI config file":
  - Create `gamestate_integration_cs2dashboard.cfg` manually as above
- Map shows `Unknown`:
  - Value depends on fields present in the current game state payload
