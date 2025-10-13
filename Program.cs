using Cs2Dashboard;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddSingleton<StatsService>();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

app.MapPost("/gsi", async (HttpRequest request, StatsService statsService) =>
{
    using var reader = new StreamReader(request.Body);
    var body = await reader.ReadToEndAsync();

    var json = JsonDocument.Parse(body);
    var player = json.RootElement.GetProperty("player");
    var name = player.GetProperty("name").GetString() ?? "Unknown";

    var matchStats = player.GetProperty("match_stats");
    var kills = matchStats.GetProperty("kills").GetInt32();
    var deaths = matchStats.GetProperty("deaths").GetInt32();
    var assists = matchStats.GetProperty("assists").GetInt32();

    statsService.Update(name, kills, deaths, assists);

    Console.WriteLine($"Updated: {name} {kills}/{deaths}/{assists}");

    return Results.Ok();
});

app.MapGet("/stats", (StatsService statsService) =>
{
    return statsService.GetAll();
});

app.MapRazorPages();

app.Urls.Clear();
app.Urls.Add("http://localhost:5050");
app.Run();