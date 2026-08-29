namespace Plugin.Maui.DeviceInfoPlus;

/// <summary>
/// Application and device fingerprint plus hardware capability probes
/// for Android and iOS.
/// </summary>
public interface IDeviceInfoPlus
{
    /// <summary>
    /// Always <c>true</c> on Android and iOS. <c>false</c> on the
    /// <c>net10.0</c> reference assembly unless a probe is injected.
    /// </summary>
    bool IsSupported { get; }

    /// <summary>
    /// Collects a fingerprint snapshot (manufacturer, model, OS, screen,
    /// density, architecture, RAM, battery, tablet, and capability flags).
    /// </summary>
    /// <example>
    /// <code>
    /// var device = await DeviceInfoPlus.GetAsync();
    /// if (device.HasNfc)
    ///     EnableNfcCheckout();
    /// </code>
    /// </example>
    Task<DeviceFingerprint> GetAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Drops any cached hardware snapshot and collects a fresh fingerprint.
    /// </summary>
    Task<DeviceFingerprint> RefreshAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Probes one hardware capability. Uses the cached snapshot when
    /// <see cref="DeviceInfoPlusOptions.CacheHardware"/> is enabled.
    /// </summary>
    Task<bool> HasAsync(DeviceCapability capability, CancellationToken cancellationToken = default);
}
