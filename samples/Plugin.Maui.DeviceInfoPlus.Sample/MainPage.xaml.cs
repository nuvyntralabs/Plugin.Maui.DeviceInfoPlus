using Plugin.Maui.DeviceInfoPlus;

namespace Plugin.Maui.DeviceInfoPlus.Sample;

public partial class MainPage : ContentPage
{
    readonly IDeviceInfoPlus _device;

    public MainPage(IDeviceInfoPlus device)
    {
        InitializeComponent();
        _device = device;
        StatusLabel.Text = $"Supported={_device.IsSupported}";
    }

    async void OnGetClicked(object? sender, EventArgs e) =>
        await ShowAsync(() => _device.GetAsync());

    async void OnRefreshClicked(object? sender, EventArgs e) =>
        await ShowAsync(() => _device.RefreshAsync());

    async Task ShowAsync(Func<Task<DeviceFingerprint>> collect)
    {
        try
        {
            var snapshot = await collect();
            StatusLabel.Text =
                $"{snapshot.Manufacturer} {snapshot.Model} · {snapshot.Os} {snapshot.OsVersion} · RAM {snapshot.Ram} MB · battery {snapshot.Battery?.ToString() ?? "n/a"}%";
            CapabilitiesLabel.Text =
                $"NFC={snapshot.HasNfc}  Bluetooth={snapshot.HasBluetooth}  Camera={snapshot.HasCamera}{Environment.NewLine}" +
                $"Biometric={snapshot.HasBiometric}  GPS={snapshot.HasGps}  Flash={snapshot.HasFlash}  Tablet={snapshot.IsTablet}";
            JsonLabel.Text = snapshot.ToJson();
        }
        catch (Exception ex)
        {
            StatusLabel.Text = ex.Message;
            CapabilitiesLabel.Text = string.Empty;
            JsonLabel.Text = string.Empty;
        }
    }
}
