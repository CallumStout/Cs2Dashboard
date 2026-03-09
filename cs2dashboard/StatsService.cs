namespace Cs2Dashboard;

public sealed class StatsService
{
    private readonly Dictionary<string, PlayerStats> _stats = new(StringComparer.OrdinalIgnoreCase);
    private readonly object _sync = new();

    public event Action<IReadOnlyList<PlayerStats>>? StatsChanged;

    public void Update(string name, int kills, int deaths, int assists)
    {
        IReadOnlyList<PlayerStats>? snapshot = null;

        lock (_sync)
        {
            if (_stats.TryGetValue(name, out var existing) &&
                existing.Kills == kills &&
                existing.Deaths == deaths &&
                existing.Assists == assists)
            {
                return;
            }

            _stats[name] = new PlayerStats
            {
                Name = name,
                Kills = kills,
                Deaths = deaths,
                Assists = assists,
                LastUpdated = DateTimeOffset.Now
            };

            snapshot = BuildSnapshot();
        }

        if (snapshot is not null)
        {
            StatsChanged?.Invoke(snapshot);
        }
    }

    public IReadOnlyList<PlayerStats> GetAll()
    {
        lock (_sync)
        {
            return BuildSnapshot();
        }
    }

    private List<PlayerStats> BuildSnapshot()
    {
        return _stats.Values
            .OrderBy(player => player.Name, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }
}

public sealed class PlayerStats
{
    public string Name { get; set; } = string.Empty;

    public int Kills { get; set; }

    public int Deaths { get; set; }

    public int Assists { get; set; }

    public DateTimeOffset LastUpdated { get; set; } = DateTimeOffset.Now;

    public string LastUpdatedDisplay => LastUpdated.LocalDateTime.ToString("HH:mm:ss");
}
