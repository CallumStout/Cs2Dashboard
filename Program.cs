using System.Text.Json;
using Avalonia;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Cs2Dashboard;

internal static class Program
{
    private static WebApplication? _webApp;

    public static StatsService StatsService { get; } = new();

    public static MainWindowViewModel MainViewModel { get; } = new(StatsService);

    [STAThread]
    public static void Main(string[] args)
    {
        StartGsiServer(args);

        try
        {
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }
        finally
        {
            MainViewModel.Dispose();
            StopGsiServer();
        }
    }

    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace();
    }

    private static void StartGsiServer(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddSingleton(StatsService);

        var app = builder.Build();

        app.MapPost("/", (HttpRequest request, StatsService statsService) => ProcessStatsPayload(request, statsService));
        app.MapPost("/gsi", (HttpRequest request, StatsService statsService) => ProcessStatsPayload(request, statsService));

        app.MapGet("/stats", (StatsService statsService) => statsService.GetAll());

        app.Urls.Clear();
        app.Urls.Add("http://localhost:3000");
        app.Urls.Add("http://localhost:5050");

        _webApp = app;
        _ = app.RunAsync();
    }

    private static async Task<IResult> ProcessStatsPayload(HttpRequest request, StatsService statsService)
    {
        using var reader = new StreamReader(request.Body);
        var body = await reader.ReadToEndAsync();

        if (string.IsNullOrWhiteSpace(body))
        {
            return Results.BadRequest("Empty body");
        }

        try
        {
            using var json = JsonDocument.Parse(body);

            if (!UpdateStatsFromPayload(json.RootElement, statsService))
            {
                return Results.BadRequest("Missing player payload");
            }

            return Results.Ok();
        }
        catch (JsonException)
        {
            return Results.BadRequest("Invalid JSON");
        }
    }

    private static void StopGsiServer()
    {
        if (_webApp is null)
        {
            return;
        }

        _webApp.StopAsync().GetAwaiter().GetResult();
        _webApp.DisposeAsync().AsTask().GetAwaiter().GetResult();
        _webApp = null;
    }

    private static bool UpdateStatsFromPayload(JsonElement root, StatsService statsService)
    {
        var foundPlayer = false;

        if (root.TryGetProperty("allplayers", out var allPlayers) && allPlayers.ValueKind == JsonValueKind.Object)
        {
            foreach (var player in allPlayers.EnumerateObject())
            {
                if (TryExtractPlayerStats(player.Value, out var name, out var kills, out var deaths, out var assists))
                {
                    statsService.Update(name, kills, deaths, assists);
                    foundPlayer = true;
                }
            }
        }

        if (root.TryGetProperty("player", out var singlePlayer))
        {
            if (TryExtractPlayerStats(singlePlayer, out var name, out var kills, out var deaths, out var assists))
            {
                statsService.Update(name, kills, deaths, assists);
                foundPlayer = true;
            }
        }

        return foundPlayer;
    }

    private static bool TryExtractPlayerStats(
        JsonElement player,
        out string name,
        out int kills,
        out int deaths,
        out int assists)
    {
        name = "Unknown";
        kills = 0;
        deaths = 0;
        assists = 0;

        if (player.TryGetProperty("name", out var nameProperty))
        {
            name = nameProperty.GetString() ?? "Unknown";
        }

        if (!player.TryGetProperty("match_stats", out var matchStats))
        {
            return false;
        }

        if (matchStats.TryGetProperty("kills", out var killsProperty))
        {
            kills = killsProperty.GetInt32();
        }

        if (matchStats.TryGetProperty("deaths", out var deathsProperty))
        {
            deaths = deathsProperty.GetInt32();
        }

        if (matchStats.TryGetProperty("assists", out var assistsProperty))
        {
            assists = assistsProperty.GetInt32();
        }

        return true;
    }
}
