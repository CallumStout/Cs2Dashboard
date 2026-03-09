using Avalonia;
using CounterStrike2GSI;
using Cs2Dashboard.ViewModels;

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

    public static void StartGsiListener()
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
            _listener = null;
        }

        MainViewModel.SetGsiConfigStatus(GsiConfigStatusMessage);
    }

    public static void StopGsiListener()
    {
        if (_listener is null)
        {
            return;
        }

        _listener.NewGameState -= OnNewGameState;
        _listener = null;
    }

    private static void OnNewGameState(GameState gameState)
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

        var name = (player.Name ?? string.Empty).Trim();
        var kills = player.MatchStats.Kills;
        var deaths = player.MatchStats.Deaths;
        var assists = player.MatchStats.Assists;

        if (string.IsNullOrWhiteSpace(name) ||
            name.Equals("unknown", StringComparison.OrdinalIgnoreCase) ||
            kills < 0 ||
            deaths < 0 ||
            assists < 0)
        {
            return false;
        }

        StatsService.Update(name, kills, deaths, assists);
        return true;
    }
}
