using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Styling;
using Avalonia.Threading;

namespace Cs2Dashboard.ViewModels;

public sealed class MainWindowViewModel : INotifyPropertyChanged, IDisposable
{
    private readonly StatsService _statsService;
    private string _playerCountLabel = "Players tracked: 0";
    private string _currentThemeLabel = "Light";
    private string _gsiConfigStatusLabel = Program.GsiConfigStatusMessage;
    private string _currentMap = "Unknown";
    private int _roundTotalDamage = -1;

    public MainWindowViewModel(StatsService statsService)
    {
        _statsService = statsService;
        _statsService.StatsChanged += OnStatsChanged;

        ReplacePlayers(_statsService.GetAll());
        UpdateThemeLabelFromApp();
    }

    public ObservableCollection<PlayerStats> Players { get; } = new();

    public string EndpointLabel { get; } = "Listening for CS2 GSI on http://localhost:3000/";

    public string GsiConfigStatusLabel
    {
        get => _gsiConfigStatusLabel;
        private set => SetField(ref _gsiConfigStatusLabel, value);
    }

    public string PlayerCountLabel
    {
        get => _playerCountLabel;
        private set => SetField(ref _playerCountLabel, value);
    }

    public string CurrentThemeLabel
    {
        get => _currentThemeLabel;
        private set => SetField(ref _currentThemeLabel, value);
    }

    public string CurrentMap
    {
        get => _currentMap;
        private set => SetField(ref _currentMap, value);
    }

    public int RoundTotalDamage
    {
        get => _roundTotalDamage;
        private set
        {
            if (!SetField(ref _roundTotalDamage, value))
            {
                return;
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RoundTotalDamageDisplay)));
        }
    }

    public string RoundTotalDamageDisplay => RoundTotalDamage < 0 ? "N/A" : RoundTotalDamage.ToString();

    public void SetCurrentMap(string map)
    {
        var next = string.IsNullOrWhiteSpace(map) ? "Unknown" : map.Trim();
        Dispatcher.UIThread.Post(() => CurrentMap = next);
    }

    public void SetRoundTotalDamage(int value)
    {
        Dispatcher.UIThread.Post(() => RoundTotalDamage = value);
    }

    public string ThemeButtonLabel => $"Switch to {(CurrentThemeLabel == "Dark" ? "Light" : "Dark")} Mode";

    public event PropertyChangedEventHandler? PropertyChanged;

    public void Dispose()
    {
        _statsService.StatsChanged -= OnStatsChanged;
    }

    public void SetGsiConfigStatus(string message)
    {
        Dispatcher.UIThread.Post(() => GsiConfigStatusLabel = message);
    }

    public void UpdateThemeLabelFromApp()
    {
        var variant = Application.Current?.RequestedThemeVariant;
        CurrentThemeLabel = variant == ThemeVariant.Dark ? "Dark" : "Light";
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ThemeButtonLabel)));
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

    private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        return true;
    }
}
