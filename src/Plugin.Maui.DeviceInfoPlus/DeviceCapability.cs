namespace Plugin.Maui.DeviceInfoPlus;

/// <summary>
/// Hardware capability that can be probed for feature targeting
/// and compatibility checks.
/// </summary>
public enum DeviceCapability
{
    /// <summary>Near-field communication.</summary>
    Nfc = 0,

    /// <summary>Classic Bluetooth and/or BLE radio.</summary>
    Bluetooth = 1,

    /// <summary>At least one camera.</summary>
    Camera = 2,

    /// <summary>Fingerprint, face, or iris hardware (enrollment not required).</summary>
    Biometric = 3,

    /// <summary>GNSS / GPS hardware.</summary>
    Gps = 4,

    /// <summary>Camera flash or torch.</summary>
    Flash = 5
}
