using Microsoft.Maui.Hosting;

namespace Plugin.Maui.DeviceInfoPlus;

sealed class DeviceInfoPlusInitializer : IMauiInitializeService
{
    public void Initialize(IServiceProvider services)
    {
        var device = services.GetService<IDeviceInfoPlus>() ?? DeviceInfoPlus.Current;
        DeviceInfoPlus.SetDefault(device);
    }
}
