using System.Globalization;
using System.Text;
using System.Text.Json;

namespace Plugin.Maui.DeviceInfoPlus;

/// <summary>
/// Point-in-time application/device fingerprint and hardware capabilities.
/// This is a telemetry and compatibility snapshot, not MAUI
/// <c>DeviceInfo</c> and not a stable device id.
/// </summary>
public sealed class DeviceFingerprint
{
    /// <summary>
    /// Initializes a fingerprint snapshot.
    /// </summary>
    public DeviceFingerprint(
        string manufacturer,
        string model,
        string os,
        string osVersion,
        string screen,
        double density,
        string architecture,
        int ram,
        int? battery,
        bool isTablet,
        bool hasNfc,
        bool hasBluetooth,
        bool hasCamera,
        bool hasBiometric,
        bool hasGps,
        bool hasFlash,
        int screenWidth = 0,
        int screenHeight = 0,
        DateTimeOffset? capturedAt = null)
    {
        Manufacturer = manufacturer ?? string.Empty;
        Model = model ?? string.Empty;
        Os = os ?? string.Empty;
        OsVersion = osVersion ?? string.Empty;
        Screen = screen ?? string.Empty;
        Density = density;
        Architecture = architecture ?? string.Empty;
        Ram = ram;
        Battery = battery;
        IsTablet = isTablet;
        HasNfc = hasNfc;
        HasBluetooth = hasBluetooth;
        HasCamera = hasCamera;
        HasBiometric = hasBiometric;
        HasGps = hasGps;
        HasFlash = hasFlash;
        ScreenWidth = screenWidth;
        ScreenHeight = screenHeight;
        CapturedAt = capturedAt ?? DateTimeOffset.UtcNow;
    }

    /// <summary>OEM name, for example <c>Samsung</c> or <c>Apple</c>.</summary>
    public string Manufacturer { get; }

    /// <summary>Hardware model, for example <c>SM-S928B</c> or <c>iPhone16,2</c>.</summary>
    public string Model { get; }

    /// <summary>Operating system family: <c>Android</c> or <c>iOS</c>.</summary>
    public string Os { get; }

    /// <summary>OS release string, for example <c>15</c> or <c>18.2</c>.</summary>
    public string OsVersion { get; }

    /// <summary>Native pixel size as <c>widthxheight</c>, for example <c>1440x3120</c>.</summary>
    public string Screen { get; }

    /// <summary>Logical density / scale factor, for example <c>3.5</c>.</summary>
    public double Density { get; }

    /// <summary>Normalized CPU ABI, for example <c>arm64</c>.</summary>
    public string Architecture { get; }

    /// <summary>Approximate total RAM in megabytes, for example <c>8192</c>.</summary>
    public int Ram { get; }

    /// <summary>Battery charge percent at capture time, or <c>null</c> when unknown.</summary>
    public int? Battery { get; }

    /// <summary>Whether the form factor is a tablet (Android sw600dp or iPad).</summary>
    public bool IsTablet { get; }

    /// <summary>Whether NFC hardware is present.</summary>
    public bool HasNfc { get; }

    /// <summary>Whether a Bluetooth radio is present.</summary>
    public bool HasBluetooth { get; }

    /// <summary>Whether at least one camera is present.</summary>
    public bool HasCamera { get; }

    /// <summary>Whether biometric hardware is present (enrollment not required).</summary>
    public bool HasBiometric { get; }

    /// <summary>Whether GNSS / GPS hardware is present.</summary>
    public bool HasGps { get; }

    /// <summary>Whether a camera flash or torch is present.</summary>
    public bool HasFlash { get; }

    /// <summary>Native screen width in pixels.</summary>
    public int ScreenWidth { get; }

    /// <summary>Native screen height in pixels.</summary>
    public int ScreenHeight { get; }

    /// <summary>UTC time the snapshot was captured.</summary>
    public DateTimeOffset CapturedAt { get; }

    /// <summary>
    /// Returns whether <paramref name="capability"/> is present on this snapshot.
    /// </summary>
    public bool Has(DeviceCapability capability) => capability switch
    {
        DeviceCapability.Nfc => HasNfc,
        DeviceCapability.Bluetooth => HasBluetooth,
        DeviceCapability.Camera => HasCamera,
        DeviceCapability.Biometric => HasBiometric,
        DeviceCapability.Gps => HasGps,
        DeviceCapability.Flash => HasFlash,
        _ => false
    };

    /// <summary>
    /// Serializes the fingerprint to camelCase JSON for telemetry,
    /// diagnostics, and support tickets.
    /// </summary>
    /// <example>
    /// <code>
    /// var device = await DeviceInfoPlus.GetAsync();
    /// var json = device.ToJson();
    /// </code>
    /// </example>
    public string ToJson()
    {
        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream))
        {
            writer.WriteStartObject();
            writer.WriteString("manufacturer", Manufacturer);
            writer.WriteString("model", Model);
            writer.WriteString("os", Os);
            writer.WriteString("osVersion", OsVersion);
            writer.WriteString("screen", Screen);
            writer.WriteNumber("density", Density);
            writer.WriteString("architecture", Architecture);
            writer.WriteNumber("ram", Ram);
            if (Battery is { } battery)
                writer.WriteNumber("battery", battery);
            else
                writer.WriteNull("battery");
            writer.WriteBoolean("isTablet", IsTablet);
            writer.WriteBoolean("hasNfc", HasNfc);
            writer.WriteBoolean("hasBluetooth", HasBluetooth);
            writer.WriteBoolean("hasCamera", HasCamera);
            writer.WriteBoolean("hasBiometric", HasBiometric);
            writer.WriteBoolean("hasGps", HasGps);
            writer.WriteBoolean("hasFlash", HasFlash);
            writer.WriteEndObject();
        }

        return Encoding.UTF8.GetString(stream.ToArray());
    }

    /// <summary>
    /// Flat property bag for attaching the snapshot to a telemetry event.
    /// </summary>
    public IReadOnlyDictionary<string, object?> ToDictionary() => new Dictionary<string, object?>
    {
        ["manufacturer"] = Manufacturer,
        ["model"] = Model,
        ["os"] = Os,
        ["osVersion"] = OsVersion,
        ["screen"] = Screen,
        ["density"] = Density,
        ["architecture"] = Architecture,
        ["ram"] = Ram,
        ["battery"] = Battery,
        ["isTablet"] = IsTablet,
        ["hasNfc"] = HasNfc,
        ["hasBluetooth"] = HasBluetooth,
        ["hasCamera"] = HasCamera,
        ["hasBiometric"] = HasBiometric,
        ["hasGps"] = HasGps,
        ["hasFlash"] = HasFlash
    };

    internal DeviceFingerprint WithBattery(int? battery, DateTimeOffset capturedAt) =>
        new(
            Manufacturer,
            Model,
            Os,
            OsVersion,
            Screen,
            Density,
            Architecture,
            Ram,
            battery,
            IsTablet,
            HasNfc,
            HasBluetooth,
            HasCamera,
            HasBiometric,
            HasGps,
            HasFlash,
            ScreenWidth,
            ScreenHeight,
            capturedAt);

    /// <inheritdoc />
    public override string ToString() =>
        string.Create(CultureInfo.InvariantCulture, $"{Manufacturer} {Model} {Os} {OsVersion} {Screen}");
}
