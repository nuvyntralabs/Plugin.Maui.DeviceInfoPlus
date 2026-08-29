namespace Plugin.Maui.DeviceInfoPlus;

/// <summary>
/// Entry point for DeviceInfoPlus when dependency injection is not used.
/// </summary>
public static class DeviceInfoPlus
{
    static IDeviceInfoPlus? _current;

    /// <summary>
    /// Gets the shared <see cref="IDeviceInfoPlus"/> instance.
    /// </summary>
    public static IDeviceInfoPlus Current => _current ??= Create(new DeviceInfoPlusOptions());

    /// <summary>
    /// Always <c>true</c> on Android and iOS.
    /// </summary>
    public static bool IsSupported => Current.IsSupported;

    /// <summary>
    /// Collects a fingerprint snapshot.
    /// </summary>
    /// <example>
    /// <code>
    /// var device = await DeviceInfoPlus.GetAsync();
    /// </code>
    /// </example>
    public static Task<DeviceFingerprint> GetAsync(CancellationToken cancellationToken = default) =>
        Current.GetAsync(cancellationToken);

    /// <summary>
    /// Drops any cached hardware snapshot and collects a fresh fingerprint.
    /// </summary>
    public static Task<DeviceFingerprint> RefreshAsync(CancellationToken cancellationToken = default) =>
        Current.RefreshAsync(cancellationToken);

    /// <summary>
    /// Probes one hardware capability.
    /// </summary>
    public static Task<bool> HasAsync(DeviceCapability capability, CancellationToken cancellationToken = default) =>
        Current.HasAsync(capability, cancellationToken);

    /// <summary>
    /// Creates a fingerprint client for the current platform.
    /// </summary>
    public static IDeviceInfoPlus Create(DeviceInfoPlusOptions? options = null)
    {
        options ??= new DeviceInfoPlusOptions();
        return new DeviceInfoPlusImplementation(options, CreatePlatform());
    }

    /// <summary>
    /// Replaces the shared instance. Intended for tests and custom implementations.
    /// </summary>
    public static void SetDefault(IDeviceInfoPlus implementation) =>
        _current = implementation ?? throw new ArgumentNullException(nameof(implementation));

    internal static DeviceInfoPlusImplementation Create(DeviceInfoPlusOptions options, IDeviceProbe probe) =>
        new(options, probe);

    internal static IDeviceProbe CreatePlatform()
    {
#if ANDROID
        return new AndroidDeviceProbe();
#elif IOS
        return new IosDeviceProbe();
#else
        return new UnsupportedDeviceProbe();
#endif
    }
}
