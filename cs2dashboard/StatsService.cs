namespace Cs2Dashboard;

public sealed class StatsService
{
    private readonly Dictionary<string, PlayerStats> _stats = new(StringComparer.OrdinalIgnoreCase);
    private readonly object _sync = new();

    public event Action<IReadOnlyList<PlayerStats>>? StatsChanged;

    public void Update(string name, int kills, int deaths, int assists)
    {
        IReadOnlyList<PlayerStats> snapshot;

        lock (_sync)
        {
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

        StatsChanged?.Invoke(snapshot);
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
            .OrderByDescending(player => player.LastUpdated)
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
