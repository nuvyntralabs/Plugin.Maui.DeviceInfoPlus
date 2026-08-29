namespace Plugin.Maui.DeviceInfoPlus;

sealed class UnsupportedDeviceProbe : IDeviceProbe
{
    public bool IsSupported => false;

    public Task<DeviceFingerprint> CollectAsync(DeviceInfoPlusOptions options, CancellationToken cancellationToken) =>
        Task.FromException<DeviceFingerprint>(CreateNotSupported());

    public Task<int?> ReadBatteryAsync(CancellationToken cancellationToken) =>
        Task.FromException<int?>(CreateNotSupported());

    static DeviceInfoPlusException CreateNotSupported() =>
        new(
            DeviceInfoPlusError.NotSupported,
            "DeviceInfoPlus is supported on Android and iOS. The net10.0 reference assembly is for tests; inject IDeviceProbe.");
}
