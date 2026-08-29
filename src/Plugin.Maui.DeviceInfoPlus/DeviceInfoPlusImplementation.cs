namespace Plugin.Maui.DeviceInfoPlus;

sealed class DeviceInfoPlusImplementation : IDeviceInfoPlus
{
    readonly DeviceInfoPlusOptions _options;
    readonly IDeviceProbe _probe;
    readonly object _gate = new();
    DeviceFingerprint? _hardware;

    public DeviceInfoPlusImplementation(DeviceInfoPlusOptions options, IDeviceProbe probe)
    {
        _options = options;
        _probe = probe;
    }

    public bool IsSupported => _probe.IsSupported;

    public Task<DeviceFingerprint> GetAsync(CancellationToken cancellationToken = default) =>
        CollectAsync(forceRefresh: false, cancellationToken);

    public Task<DeviceFingerprint> RefreshAsync(CancellationToken cancellationToken = default) =>
        CollectAsync(forceRefresh: true, cancellationToken);

    public async Task<bool> HasAsync(DeviceCapability capability, CancellationToken cancellationToken = default)
    {
        var device = await GetAsync(cancellationToken).ConfigureAwait(false);
        return device.Has(capability);
    }

    async Task<DeviceFingerprint> CollectAsync(bool forceRefresh, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        EnsureSupported();

        DeviceFingerprint? cached;
        lock (_gate)
            cached = forceRefresh ? null : _hardware;

        if (cached is not null && _options.CacheHardware)
        {
            var battery = _options.IncludeBattery
                ? await _probe.ReadBatteryAsync(cancellationToken).ConfigureAwait(false)
                : null;
            return cached.WithBattery(battery, DateTimeOffset.UtcNow);
        }

        var snapshot = await _probe.CollectAsync(_options, cancellationToken).ConfigureAwait(false);
        if (!_options.IncludeBattery)
            snapshot = snapshot.WithBattery(null, snapshot.CapturedAt);

        if (_options.CacheHardware)
        {
            lock (_gate)
                _hardware = snapshot;
        }

        return snapshot;
    }

    void EnsureSupported()
    {
        if (_probe.IsSupported)
            return;

        throw new DeviceInfoPlusException(
            DeviceInfoPlusError.NotSupported,
            "DeviceInfoPlus is supported on Android and iOS. The net10.0 reference assembly is for tests; inject IDeviceProbe.");
    }
}
