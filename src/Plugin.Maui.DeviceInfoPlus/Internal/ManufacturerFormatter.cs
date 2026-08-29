using System.Globalization;

namespace Plugin.Maui.DeviceInfoPlus;

static class ManufacturerFormatter
{
    public static string Format(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return "Unknown";

        return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(value.Trim().ToLowerInvariant());
    }
}
