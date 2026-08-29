using System.Runtime.InteropServices;

namespace Plugin.Maui.DeviceInfoPlus;

static class ArchitectureMapper
{
    public static string FromAbi(string? abi)
    {
        if (string.IsNullOrWhiteSpace(abi))
            return FromProcess();

        var value = abi.Trim().ToLowerInvariant();
        if (value is "arm64-v8a" or "arm64" or "aarch64")
            return "arm64";
        if (value is "armeabi-v7a" or "armeabi" or "arm")
            return "arm";
        if (value is "x86_64" or "x64" or "amd64")
            return "x64";
        if (value is "x86" or "i386" or "i686")
            return "x86";

        return value;
    }

    public static string FromProcess() => RuntimeInformation.ProcessArchitecture switch
    {
        Architecture.Arm64 => "arm64",
        Architecture.Arm => "arm",
        Architecture.X64 => "x64",
        Architecture.X86 => "x86",
        _ => RuntimeInformation.ProcessArchitecture.ToString().ToLowerInvariant()
    };
}
