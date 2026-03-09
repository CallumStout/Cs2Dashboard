using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Styling;
using Cs2Dashboard.ViewModels;

namespace Cs2Dashboard;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void OnToggleThemeClicked(object? sender, RoutedEventArgs e)
    {
        var app = Application.Current;
        if (app is null)
        {
            return;
        }

        var isDark = app.RequestedThemeVariant == ThemeVariant.Dark;
        app.RequestedThemeVariant = isDark ? ThemeVariant.Light : ThemeVariant.Dark;

        if (DataContext is MainWindowViewModel vm)
        {
            vm.UpdateThemeLabelFromApp();
        }
    }
}
