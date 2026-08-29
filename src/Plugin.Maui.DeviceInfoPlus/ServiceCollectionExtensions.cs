namespace Plugin.Maui.DeviceInfoPlus;

/// <summary>
/// Registers DeviceInfoPlus services without MAUI lifecycle hooks.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds <see cref="IDeviceInfoPlus"/> using the supplied options instance.
    /// </summary>
    public static IServiceCollection AddDeviceInfoPlus(this IServiceCollection services, DeviceInfoPlusOptions options)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(options);

        services.AddSingleton(options);
        services.TryAddSingleton<IDeviceInfoPlus>(sp =>
        {
            var resolved = sp.GetService<DeviceInfoPlusOptions>() ?? options;
            var probe = sp.GetService<IDeviceProbe>() ?? DeviceInfoPlus.CreatePlatform();
            var device = DeviceInfoPlus.Create(resolved, probe);
            DeviceInfoPlus.SetDefault(device);
            return device;
        });

        return services;
    }

    /// <summary>
    /// Adds <see cref="IDeviceInfoPlus"/> and applies <paramref name="configure"/> to a new options instance.
    /// </summary>
    public static IServiceCollection AddDeviceInfoPlus(this IServiceCollection services, Action<DeviceInfoPlusOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        var options = new DeviceInfoPlusOptions();
        configure?.Invoke(options);
        return services.AddDeviceInfoPlus(options);
    }
}
