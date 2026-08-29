using Microsoft.Maui.Hosting;

namespace Plugin.Maui.DeviceInfoPlus;

/// <summary>
/// MAUI host registration for DeviceInfoPlus.
/// </summary>
public static class MauiAppBuilderExtensions
{
    /// <summary>
    /// Registers <see cref="IDeviceInfoPlus"/> as a singleton.
    /// </summary>
    /// <example>
    /// <code>
    /// builder.UseDeviceInfoPlus(options =>
    /// {
    ///     options.IncludeBattery = true;
    ///     options.CacheHardware = true;
    /// });
    /// </code>
    /// </example>
    public static MauiAppBuilder UseDeviceInfoPlus(this MauiAppBuilder builder, Action<DeviceInfoPlusOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        var options = new DeviceInfoPlusOptions();
        configure?.Invoke(options);

        builder.Services.AddDeviceInfoPlus(options);
        builder.Services.AddTransient<IMauiInitializeService, DeviceInfoPlusInitializer>();
        return builder;
    }
}
