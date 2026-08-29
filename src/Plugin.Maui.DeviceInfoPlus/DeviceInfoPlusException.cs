namespace Plugin.Maui.DeviceInfoPlus;

/// <summary>
/// Thrown when a device fingerprint cannot be collected.
/// </summary>
public sealed class DeviceInfoPlusException : Exception
{
    /// <summary>
    /// Initializes a new exception with an error code and message.
    /// </summary>
    public DeviceInfoPlusException(DeviceInfoPlusError error, string message, Exception? innerException = null)
        : base(message, innerException)
    {
        Error = error;
    }

    /// <summary>
    /// Gets the classified error.
    /// </summary>
    public DeviceInfoPlusError Error { get; }
}
