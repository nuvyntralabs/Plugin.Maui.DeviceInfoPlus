namespace Plugin.Maui.DeviceInfoPlus.Tests;

sealed class FakeDeviceProbe : IDeviceProbe
{
    public bool IsSupported { get; set; } = true;

    public DeviceFingerprint Snapshot { get; set; } = new(
        manufacturer: "Samsung",
        model: "SM-S928B",
        os: "Android",
        osVersion: "15",
        screen: "1440x3120",
        density: 3.5,
        architecture: "arm64",
        ram: 8192,
        battery: 78,
        isTablet: false,
        hasNfc: true,
        hasBluetooth: true,
        hasCamera: true,
        hasBiometric: true,
        hasGps: true,
        hasFlash: true,
        screenWidth: 1440,
        screenHeight: 3120);

    public int? NextBattery { get; set; }

    public int CollectCount { get; private set; }

    public int BatteryReadCount { get; private set; }

    public Task<DeviceFingerprint> CollectAsync(DeviceInfoPlusOptions options, CancellationToken cancellationToken)
    {
        CollectCount++;
        var battery = options.IncludeBattery ? Snapshot.Battery : null;
        return Task.FromResult(Snapshot.WithBattery(battery, DateTimeOffset.UtcNow));
    }

    public Task<int?> ReadBatteryAsync(CancellationToken cancellationToken)
    {
        BatteryReadCount++;
        return Task.FromResult(NextBattery ?? Snapshot.Battery);
    }
}

static class Harness
{
    public static (DeviceInfoPlusImplementation Device, FakeDeviceProbe Probe) Create(
        Action<DeviceInfoPlusOptions>? configure = null)
    {
        var options = new DeviceInfoPlusOptions();
        configure?.Invoke(options);
        var probe = new FakeDeviceProbe();
        var device = DeviceInfoPlus.Create(options, probe);
        return (device, probe);
    }
}
