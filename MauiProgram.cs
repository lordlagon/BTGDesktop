namespace BTGDesktop;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .RegisterServices()
            .RegisterPageViewModel()
            .UseSkiaSharp()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
    public static MauiAppBuilder RegisterServices(this MauiAppBuilder builder)
    {
        builder.Services.AddSingleton<ISimulatorService, SimulatorService>();
        return builder;
    }
    public static MauiAppBuilder RegisterPageViewModel(this MauiAppBuilder builder)
    {
        builder.Services.AddTransient<SimulatorPage>();
        builder.Services.AddTransient<SimulatorViewModel>();
        return builder;
    }
}