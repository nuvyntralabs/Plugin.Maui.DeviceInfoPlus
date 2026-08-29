namespace Plugin.Maui.DeviceInfoPlus;

interface IDeviceProbe
{
    bool IsSupported { get; }

    Task<DeviceFingerprint> CollectAsync(DeviceInfoPlusOptions options, CancellationToken cancellationToken);

    Task<int?> ReadBatteryAsync(CancellationToken cancellationToken);
}
