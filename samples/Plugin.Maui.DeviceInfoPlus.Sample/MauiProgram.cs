using Microsoft.Extensions.Logging;
using Plugin.Maui.DeviceInfoPlus;

namespace Plugin.Maui.DeviceInfoPlus.Sample;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.Services.AddSingleton<MainPage>();

        builder
            .UseMauiApp<App>()
            .UseDeviceInfoPlus(options =>
            {
                options.IncludeBattery = true;
                options.CacheHardware = true;
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
