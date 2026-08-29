#if ANDROID
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using MauiPlatform = Microsoft.Maui.ApplicationModel.Platform;

namespace Plugin.Maui.DeviceInfoPlus;

sealed class AndroidDeviceProbe : IDeviceProbe
{
    public bool IsSupported => true;

    public Task<DeviceFingerprint> CollectAsync(DeviceInfoPlusOptions options, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var context = MauiPlatform.AppContext;
        var pm = context.PackageManager;
        GetScreen(context, out var width, out var height, out var density);
        var isTablet = (context.Resources?.Configuration?.SmallestScreenWidthDp ?? 0) >= 600;

        var fingerprint = new DeviceFingerprint(
            manufacturer: ManufacturerFormatter.Format(Build.Manufacturer),
            model: Build.Model ?? "Unknown",
            os: "Android",
            osVersion: Build.VERSION.Release ?? Build.VERSION.SdkInt.ToString(),
            screen: $"{width}x{height}",
            density: density,
            architecture: ArchitectureMapper.FromAbi(FirstAbi()),
            ram: ReadRamMb(context),
            battery: options.IncludeBattery ? ReadBattery(context) : null,
            isTablet: isTablet,
            hasNfc: HasFeature(pm, PackageManager.FeatureNfc),
            hasBluetooth: HasFeature(pm, PackageManager.FeatureBluetooth) || HasFeature(pm, PackageManager.FeatureBluetoothLe),
            hasCamera: HasFeature(pm, PackageManager.FeatureCameraAny) || HasFeature(pm, PackageManager.FeatureCamera),
            hasBiometric: HasBiometric(pm),
            hasGps: HasFeature(pm, PackageManager.FeatureLocationGps),
            hasFlash: HasFeature(pm, PackageManager.FeatureCameraFlash),
            screenWidth: width,
            screenHeight: height,
            capturedAt: DateTimeOffset.UtcNow);

        return Task.FromResult(fingerprint);
    }

    public Task<int?> ReadBatteryAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(ReadBattery(MauiPlatform.AppContext));
    }

    static string? FirstAbi()
    {
        try
        {
            return Build.SupportedAbis is { Count: > 0 } abis
                ? abis[0]
                : Build.Supported64BitAbis is { Count: > 0 } x64
                    ? x64[0]
                    : null;
        }
        catch
        {
            return null;
        }
    }

    static void GetScreen(Context context, out int width, out int height, out double density)
    {
        width = 0;
        height = 0;
        density = 1;

        try
        {
            if (OperatingSystem.IsAndroidVersionAtLeast(30)
                && context.GetSystemService(Context.WindowService) is IWindowManager windowManager)
            {
                var bounds = windowManager.CurrentWindowMetrics.Bounds;
                width = bounds.Width();
                height = bounds.Height();
            }
        }
        catch
        {
            // Fall back to DisplayMetrics.
        }

        var metrics = context.Resources?.DisplayMetrics;
        if (metrics is null)
            return;

        density = metrics.Density;
        if (width <= 0 || height <= 0)
        {
            width = metrics.WidthPixels;
            height = metrics.HeightPixels;
        }
    }

    static int ReadRamMb(Context context)
    {
        try
        {
            if (context.GetSystemService(Context.ActivityService) is not ActivityManager manager)
                return 0;

            var info = new ActivityManager.MemoryInfo();
            manager.GetMemoryInfo(info);
            return (int)(info.TotalMem / (1024L * 1024L));
        }
        catch
        {
            return 0;
        }
    }

    static int? ReadBattery(Context context)
    {
        try
        {
            if (context.GetSystemService(Context.BatteryService) is not BatteryManager manager)
                return null;

            var capacity = manager.GetIntProperty((int)BatteryProperty.Capacity);
            return capacity is >= 0 and <= 100 ? capacity : null;
        }
        catch
        {
            return null;
        }
    }

    static bool HasBiometric(PackageManager? pm)
    {
        if (HasFeature(pm, PackageManager.FeatureFingerprint))
            return true;

        if (OperatingSystem.IsAndroidVersionAtLeast(29)
            && (HasFeature(pm, PackageManager.FeatureFace) || HasFeature(pm, PackageManager.FeatureIris)))
            return true;

        return false;
    }

    static bool HasFeature(PackageManager? pm, string feature)
    {
        try
        {
            return pm?.HasSystemFeature(feature) == true;
        }
        catch
        {
            return false;
        }
    }
}
#endif
