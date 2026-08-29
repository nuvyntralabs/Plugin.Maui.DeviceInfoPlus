using System.Text.Json;

namespace Plugin.Maui.DeviceInfoPlus.Tests;

public sealed class DeviceFingerprintTests
{
    [Fact]
    public void ToJson_matches_telemetry_shape()
    {
        var device = Sample();

        using var json = JsonDocument.Parse(device.ToJson());
        var root = json.RootElement;

        Assert.Equal("Samsung", root.GetProperty("manufacturer").GetString());
        Assert.Equal("SM-S928B", root.GetProperty("model").GetString());
        Assert.Equal("Android", root.GetProperty("os").GetString());
        Assert.Equal("15", root.GetProperty("osVersion").GetString());
        Assert.Equal("1440x3120", root.GetProperty("screen").GetString());
        Assert.Equal(3.5, root.GetProperty("density").GetDouble());
        Assert.Equal("arm64", root.GetProperty("architecture").GetString());
        Assert.Equal(8192, root.GetProperty("ram").GetInt32());
        Assert.Equal(78, root.GetProperty("battery").GetInt32());
        Assert.False(root.GetProperty("isTablet").GetBoolean());
        Assert.True(root.GetProperty("hasNfc").GetBoolean());
        Assert.True(root.GetProperty("hasBluetooth").GetBoolean());
        Assert.True(root.GetProperty("hasCamera").GetBoolean());
        Assert.True(root.GetProperty("hasBiometric").GetBoolean());
        Assert.True(root.GetProperty("hasGps").GetBoolean());
        Assert.True(root.GetProperty("hasFlash").GetBoolean());
    }

    [Fact]
    public void ToJson_writes_null_battery_when_unknown()
    {
        var device = Sample(battery: null);

        using var json = JsonDocument.Parse(device.ToJson());
        Assert.Equal(JsonValueKind.Null, json.RootElement.GetProperty("battery").ValueKind);
    }

    [Fact]
    public void Has_maps_capability_flags()
    {
        var device = Sample(hasNfc: false, hasFlash: false);

        Assert.False(device.HasNfc);
        Assert.False(device.Has(DeviceCapability.Nfc));
        Assert.True(device.Has(DeviceCapability.Gps));
        Assert.False(device.Has(DeviceCapability.Flash));
        Assert.True(device.HasBluetooth);
        Assert.True(device.HasCamera);
        Assert.True(device.HasBiometric);
    }

    [Fact]
    public void ToDictionary_is_flat_for_telemetry()
    {
        var values = Sample().ToDictionary();

        Assert.Equal("Samsung", values["manufacturer"]);
        Assert.Equal(8192, values["ram"]);
        Assert.Equal(true, values["hasGps"]);
    }

    [Fact]
    public void ArchitectureMapper_normalizes_abis()
    {
        Assert.Equal("arm64", ArchitectureMapper.FromAbi("arm64-v8a"));
        Assert.Equal("arm", ArchitectureMapper.FromAbi("armeabi-v7a"));
        Assert.Equal("x64", ArchitectureMapper.FromAbi("x86_64"));
        Assert.Equal("x86", ArchitectureMapper.FromAbi("x86"));
    }

    [Fact]
    public void ManufacturerFormatter_title_cases_oem()
    {
        Assert.Equal("Samsung", ManufacturerFormatter.Format("samsung"));
        Assert.Equal("Unknown", ManufacturerFormatter.Format("  "));
    }

    static DeviceFingerprint Sample(
        int? battery = 78,
        bool hasNfc = true,
        bool hasFlash = true) =>
        new(
            manufacturer: "Samsung",
            model: "SM-S928B",
            os: "Android",
            osVersion: "15",
            screen: "1440x3120",
            density: 3.5,
            architecture: "arm64",
            ram: 8192,
            battery: battery,
            isTablet: false,
            hasNfc: hasNfc,
            hasBluetooth: true,
            hasCamera: true,
            hasBiometric: true,
            hasGps: true,
            hasFlash: hasFlash,
            screenWidth: 1440,
            screenHeight: 3120);
}
