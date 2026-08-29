#if IOS
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using AVFoundation;
using CoreNFC;
using Foundation;
using LocalAuthentication;
using UIKit;

namespace Plugin.Maui.DeviceInfoPlus;

sealed class IosDeviceProbe : IDeviceProbe
{
    public bool IsSupported => true;

    public Task<DeviceFingerprint> CollectAsync(DeviceInfoPlusOptions options, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var screen = UIScreen.MainScreen;
        var native = screen.NativeBounds.Size;
        var width = (int)Math.Round(native.Width);
        var height = (int)Math.Round(native.Height);
        var isTablet = UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad;
        var camera = GetVideoDevice();

        var fingerprint = new DeviceFingerprint(
            manufacturer: "Apple",
            model: ReadMachine() ?? UIDevice.CurrentDevice.Model ?? "iPhone",
            os: "iOS",
            osVersion: UIDevice.CurrentDevice.SystemVersion ?? string.Empty,
            screen: $"{width}x{height}",
            density: screen.Scale,
            architecture: ArchitectureMapper.FromProcess(),
            ram: ReadRamMb(),
            battery: options.IncludeBattery ? ReadBattery() : null,
            isTablet: isTablet,
            hasNfc: ReadNfc(),
            hasBluetooth: true,
            hasCamera: camera is not null,
            hasBiometric: ReadBiometric(),
            hasGps: ReadGps(isTablet),
            hasFlash: camera?.HasFlash == true || camera?.HasTorch == true,
            screenWidth: width,
            screenHeight: height,
            capturedAt: DateTimeOffset.UtcNow);

        return Task.FromResult(fingerprint);
    }

    public Task<int?> ReadBatteryAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(ReadBattery());
    }

    static int ReadRamMb()
    {
        try
        {
            return (int)(NSProcessInfo.ProcessInfo.PhysicalMemory / (1024UL * 1024UL));
        }
        catch
        {
            return 0;
        }
    }

    static int? ReadBattery()
    {
        var device = UIDevice.CurrentDevice;
        var wasEnabled = device.BatteryMonitoringEnabled;
        try
        {
            device.BatteryMonitoringEnabled = true;
            var level = device.BatteryLevel;
            if (level < 0)
                return null;

            return (int)Math.Round(level * 100);
        }
        catch
        {
            return null;
        }
        finally
        {
            if (!wasEnabled)
                device.BatteryMonitoringEnabled = false;
        }
    }

    static bool ReadNfc()
    {
        try
        {
            return NFCNdefReaderSession.ReadingAvailable;
        }
        catch
        {
            return false;
        }
    }

    static bool ReadBiometric()
    {
        try
        {
            using var context = new LAContext();
            if (context.CanEvaluatePolicy(LAPolicy.DeviceOwnerAuthenticationWithBiometrics, out var error))
                return true;

            return error?.Code == (long)LAStatus.BiometryNotEnrolled;
        }
        catch
        {
            return false;
        }
    }

    static bool ReadGps(bool isTablet)
    {
        if (!isTablet)
            return true;

        try
        {
            foreach (var nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                var name = nic.Name ?? string.Empty;
                if (name.StartsWith("pdp_ip", StringComparison.OrdinalIgnoreCase))
                    return true;
            }
        }
        catch
        {
            // Wi-Fi iPads have no GNSS. Fail closed.
        }

        return false;
    }

    static AVCaptureDevice? GetVideoDevice()
    {
        try
        {
            return AVCaptureDevice.GetDefaultDevice(AVMediaTypes.Video);
        }
        catch
        {
            return null;
        }
    }

    static string? ReadMachine()
    {
        nuint size = 0;
        if (sysctlbyname("hw.machine", IntPtr.Zero, ref size, IntPtr.Zero, 0) != 0 || size == 0)
            return null;

        var buffer = Marshal.AllocHGlobal((int)size);
        try
        {
            return sysctlbyname("hw.machine", buffer, ref size, IntPtr.Zero, 0) == 0
                ? Marshal.PtrToStringAnsi(buffer)
                : null;
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }
    }

    [DllImport("libc", CallingConvention = CallingConvention.Cdecl)]
    static extern int sysctlbyname(string name, IntPtr oldp, ref nuint oldlenp, IntPtr newp, nuint newlen);
}
#endif
