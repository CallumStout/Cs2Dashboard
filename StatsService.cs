namespace Cs2Dashboard;

public class StatsService
{
    private readonly Dictionary<string, PlayerStats> _stats = new();

    public void Update(string name, int kills, int deaths, int assists)
    {
        _stats[name] = new PlayerStats
        {
            Name = name,
            Kills = kills,
            Deaths = deaths,
            Assists = assists,
            LastUpdated = DateTime.Now
        };
    }

    public List<PlayerStats> GetAll()
    {
        return _stats.Values.ToList();
    }
}

public class PlayerStats
{
    public string Name { get; set; } = "";

    public int Kills { get; set; }

    public int Deaths { get; set; }

    public int Assists { get; set; }

    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}
