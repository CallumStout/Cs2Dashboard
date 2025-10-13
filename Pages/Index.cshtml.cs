using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Cs2Dashboard;

public class IndexModel : PageModel
{
    private readonly StatsService _statsService;

    public IndexModel(StatsService statsService)
    {
        _statsService = statsService;
    }

    public List<PlayerStats> CurrentStats { get; set; } = new();

    public void OnGet()
    {
        CurrentStats = _statsService.GetAll();
    }
}
