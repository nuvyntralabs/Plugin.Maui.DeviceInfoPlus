using Microsoft.Extensions.DependencyInjection;

namespace Plugin.Maui.DeviceInfoPlus.Tests;

public sealed class DeviceInfoPlusTests
{
    [Fact]
    public async Task GetAsync_returns_probe_snapshot()
    {
        var (device, _) = Harness.Create();

        var snapshot = await device.GetAsync();

        Assert.Equal("Samsung", snapshot.Manufacturer);
        Assert.Equal("SM-S928B", snapshot.Model);
        Assert.Equal("Android", snapshot.Os);
        Assert.Equal("15", snapshot.OsVersion);
        Assert.Equal("1440x3120", snapshot.Screen);
        Assert.Equal(3.5, snapshot.Density);
        Assert.Equal("arm64", snapshot.Architecture);
        Assert.Equal(8192, snapshot.Ram);
        Assert.Equal(78, snapshot.Battery);
        Assert.False(snapshot.IsTablet);
        Assert.True(snapshot.HasNfc);
        Assert.True(snapshot.HasGps);
    }

    [Fact]
    public async Task GetAsync_caches_hardware_and_refreshes_battery()
    {
        var (device, probe) = Harness.Create(options => options.CacheHardware = true);
        probe.NextBattery = 41;

        var first = await device.GetAsync();
        var second = await device.GetAsync();

        Assert.Equal(1, probe.CollectCount);
        Assert.Equal(1, probe.BatteryReadCount);
        Assert.Equal(78, first.Battery);
        Assert.Equal(41, second.Battery);
        Assert.Equal(first.Model, second.Model);
        Assert.Equal(first.Ram, second.Ram);
    }

    [Fact]
    public async Task RefreshAsync_collects_again()
    {
        var (device, probe) = Harness.Create();

        await device.GetAsync();
        await device.RefreshAsync();

        Assert.Equal(2, probe.CollectCount);
    }

    [Fact]
    public async Task GetAsync_omits_battery_when_disabled()
    {
        var (device, _) = Harness.Create(options => options.IncludeBattery = false);

        var snapshot = await device.GetAsync();

        Assert.Null(snapshot.Battery);
    }

    [Fact]
    public async Task HasAsync_uses_snapshot_flags()
    {
        var (device, probe) = Harness.Create();
        probe.Snapshot = probe.Snapshot.WithBattery(78, DateTimeOffset.UtcNow);

        Assert.True(await device.HasAsync(DeviceCapability.Nfc));
        Assert.True(await device.HasAsync(DeviceCapability.Camera));
    }

    [Fact]
    public async Task HasAsync_reports_missing_capability()
    {
        var (device, probe) = Harness.Create();
        probe.Snapshot = new DeviceFingerprint(
            "Apple", "iPhone16,2", "iOS", "18.2", "1179x2556", 3,
            "arm64", 8192, 90, false,
            hasNfc: true, hasBluetooth: true, hasCamera: true,
            hasBiometric: true, hasGps: true, hasFlash: false);

        Assert.False(await device.HasAsync(DeviceCapability.Flash));
        Assert.True(await device.HasAsync(DeviceCapability.Biometric));
    }

    [Fact]
    public async Task Unsupported_probe_throws()
    {
        var device = DeviceInfoPlus.Create(new DeviceInfoPlusOptions(), new UnsupportedDeviceProbe());

        var error = await Assert.ThrowsAsync<DeviceInfoPlusException>(() => device.GetAsync());
        Assert.Equal(DeviceInfoPlusError.NotSupported, error.Error);
        Assert.False(device.IsSupported);
    }

    [Fact]
    public void Create_without_probe_is_unsupported_on_net()
    {
        var device = DeviceInfoPlus.Create();
        Assert.False(device.IsSupported);
    }

    [Fact]
    public async Task AddDeviceInfoPlus_resolves_injected_probe()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IDeviceProbe, FakeDeviceProbe>();
        services.AddDeviceInfoPlus(options => options.CacheHardware = false);

        await using var provider = services.BuildServiceProvider();
        var resolved = provider.GetRequiredService<IDeviceInfoPlus>();

        var snapshot = await resolved.GetAsync();
        Assert.Equal("SM-S928B", snapshot.Model);
        Assert.Same(resolved, DeviceInfoPlus.Current);
    }
}
