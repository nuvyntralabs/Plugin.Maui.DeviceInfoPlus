namespace Plugin.Maui.DeviceInfoPlus;

/// <summary>
/// Process-wide defaults for <see cref="IDeviceInfoPlus"/>.
/// </summary>
public sealed class DeviceInfoPlusOptions
{
    /// <summary>
    /// When <c>true</c>, <see cref="IDeviceInfoPlus.GetAsync"/> includes the
    /// current battery percent. Default is <c>true</c>.
    /// </summary>
    public bool IncludeBattery { get; set; } = true;

    /// <summary>
    /// When <c>true</c>, static hardware fields and capability flags are
    /// collected once and reused. Battery is still refreshed on each
    /// <see cref="IDeviceInfoPlus.GetAsync"/> call. Default is <c>true</c>.
    /// </summary>
    public bool CacheHardware { get; set; } = true;
}
