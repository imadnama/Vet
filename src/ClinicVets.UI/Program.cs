using Avalonia;

AppBuilder.Configure<ClinicVets.UI.App>()
    .UsePlatformDetect()
    .WithInterFont()
    .LogToTrace()
    .StartWithClassicDesktopLifetime(args);
