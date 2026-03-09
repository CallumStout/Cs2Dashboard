using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia.Threading;

namespace Cs2Dashboard.ViewModels;

public sealed class MainWindowViewModel : INotifyPropertyChanged, IDisposable
{
    private readonly StatsService _statsService;
    private string _playerCountLabel = "Players tracked: 0";

    public MainWindowViewModel(StatsService statsService)
    {
        _statsService = statsService;
        _statsService.StatsChanged += OnStatsChanged;

        ReplacePlayers(_statsService.GetAll());
    }

    public ObservableCollection<PlayerStats> Players { get; } = new();

    public string EndpointLabel { get; } = "Listening for CS2 GSI on http://localhost:5050/gsi";

    public string PlayerCountLabel
    {
        get => _playerCountLabel;
        private set => SetField(ref _playerCountLabel, value);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public void Dispose()
    {
        _statsService.StatsChanged -= OnStatsChanged;
    }

    private void OnStatsChanged(IReadOnlyList<PlayerStats> players)
    {
        Dispatcher.UIThread.Post(() => ReplacePlayers(players));
    }

    private void ReplacePlayers(IReadOnlyList<PlayerStats> players)
    {
        Players.Clear();

        foreach (var player in players)
        {
            Players.Add(player);
        }

        PlayerCountLabel = $"Players tracked: {Players.Count}";
    }

    private void SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return;
        }

        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
