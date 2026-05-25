using System;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using ClinicVets.UI.ViewModels;

namespace ClinicVets.UI.Views;

public partial class SplashWindow : Window
{
    private readonly MainViewModel _mainVm;

    public SplashWindow(MainViewModel mainVm)
    {
        _mainVm = mainVm;
        InitializeComponent();
    }

    protected override void OnOpened(EventArgs e)
    {
        base.OnOpened(e);

        Transitions = new Transitions
        {
            new DoubleTransition
            {
                Property = OpacityProperty,
                Duration = TimeSpan.FromMilliseconds(600),
                Easing   = new CubicEaseOut()
            }
        };
        Opacity = 1.0;

        DispatcherTimer.RunOnce(StartFadeOut, TimeSpan.FromMilliseconds(2000));
    }

    private void StartFadeOut()
    {
        Transitions = new Transitions
        {
            new DoubleTransition
            {
                Property = OpacityProperty,
                Duration = TimeSpan.FromMilliseconds(400),
                Easing   = new CubicEaseIn()
            }
        };
        Opacity = 0.0;

        DispatcherTimer.RunOnce(ShowMainWindow, TimeSpan.FromMilliseconds(400));
    }

    private void ShowMainWindow()
    {
        var main = new MainWindow { DataContext = _mainVm };

        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            desktop.MainWindow = main;

        main.Show();
        Close();
    }
}
