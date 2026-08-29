# Changelog

## 1.0.0

- Device fingerprint and capability service for .NET MAUI on Android and iOS
- `DeviceInfoPlus.GetAsync` returns manufacturer, model, OS, screen, density, architecture, RAM, battery, and tablet
- Capability flags: `HasNfc`, `HasBluetooth`, `HasCamera`, `HasBiometric`, `HasGps`, `HasFlash`
- `ToJson` / `ToDictionary` for telemetry, diagnostics, and support tickets
- Does not wrap MAUI `DeviceInfo`, DeviceSession identity, or AppHealth findings
- Sample app and unit tests
