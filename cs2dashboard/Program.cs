using Avalonia;
using CounterStrike2GSI;
using CounterStrike2GSI.Nodes;
using Cs2Dashboard.ViewModels;
using Cs2Dashboard.Views;

namespace Cs2Dashboard;

internal static class Program
{
    private const int GsiPort = 3000;
    private static GameStateListener? _listener;

    public static StatsService StatsService { get; } = new();
    public static string GsiConfigStatusMessage { get; private set; } = "GSI config generation has not run yet.";

    public static MainWindowViewModel MainViewModel { get; } = new(StatsService);

    [STAThread]
    public static void Main(string[] args)
    {
        StartGsiListener();

        try
        {
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }
        finally
        {
            MainViewModel.Dispose();
            StopGsiListener();
        }
    }

    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace();
    }

    private static void StartGsiListener()
    {
        try
        {
            _listener = new GameStateListener(GsiPort);
            _listener.NewGameState += OnNewGameState;
            _listener.Start();

            var generated = _listener.GenerateGSIConfigFile("cs2dashboard");
            GsiConfigStatusMessage = generated
                ? "GSI config created/updated automatically."
                : "Could not auto-create GSI config file. Create gamestate_integration_*.cfg manually.";
        }
        catch (Exception ex)
        {
            GsiConfigStatusMessage = $"Failed to start GSI listener: {ex.Message}";
        }
    }

    private static void StopGsiListener()
    {
        if (_listener is null)
        {
            return;
        }

        _listener.NewGameState -= OnNewGameState;
        _listener.Stop();
        _listener.Dispose();
        _listener = null;
    }

    private static void OnNewGameState(CounterStrike2GSI.GameState gameState)
    {
        var hasUpdatedAnyPlayer = false;

        if (gameState.AllPlayers is not null)
        {
            foreach (var playerEntry in gameState.AllPlayers)
            {
                hasUpdatedAnyPlayer |= TryUpdatePlayer(playerEntry.Value);
            }
        }

        if (!hasUpdatedAnyPlayer && gameState.Player is not null)
        {
            _ = TryUpdatePlayer(gameState.Player);
        }
    }

    private static bool TryUpdatePlayer(CounterStrike2GSI.Nodes.Player player)
    {
        if (player.MatchStats is null)
        {
            return false;
        }

        var name = string.IsNullOrWhiteSpace(player.Name) ? "Unknown" : player.Name;

        StatsService.Update(
            name,
            player.MatchStats.Kills,
            player.MatchStats.Deaths,
            player.MatchStats.Assists);

        return true;
    }
}
