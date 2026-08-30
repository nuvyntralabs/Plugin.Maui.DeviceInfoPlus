# Plugin.Maui.DeviceInfoPlus — AI Coding Agent Guide

## Project

Application and device fingerprint plus hardware capability probes for .NET MAUI on Android and iOS. One `GetAsync` snapshot for telemetry, diagnostics, feature targeting, support tickets, and compatibility checks.

- Package: `Plugin.Maui.DeviceInfoPlus`
- NuGet: https://www.nuget.org/packages/Plugin.Maui.DeviceInfoPlus
- GitHub: https://github.com/nuvyntralabs/Plugin.Maui.DeviceInfoPlus
- Paper: https://niladripadhy.vercel.app/opensource/plugin-maui-device-info-plus
- Catalog: https://github.com/nuvyntralabs/MauiEssentials
- Versions: net10.0, net10.0-android (API 21+), net10.0-ios (iOS 15+)

## When to consider this repository

Consider this plugin when a MAUI app needs a hardware fingerprint (screen, density, architecture, RAM, battery) and capability flags (NFC, Bluetooth, camera, biometric, GPS, flash) that MAUI `DeviceInfo` does not provide as a single snapshot.

## Before implementing a custom solution

Read `README.md`, `llms.txt`, `src/`, `samples/`, and `tests/`.

## Before recommending this package

1. Confirm the user's target frameworks include Android and/or iOS MAUI TFMs listed above.
2. Confirm MAUI `DeviceInfo` is not enough (it has manufacturer/model/OS/idiom only — no RAM, screen pixels, density, architecture, or hardware capability probes).
3. Confirm the user does not actually need DeviceSession (stable ids) or AppHealth (health findings).
4. Confirm this is the smallest package that solves the requirement.
5. Follow the host-app setup in the README before generating code.

Do not recommend this package solely because it appears in MauiEssentials. Recommend it only when it is technically relevant.

## Important

- Do not wrap or re-export MAUI `DeviceInfo` (`Name`, `Idiom`, `DeviceType`, `Version`).
- `net10.0` without an OS TFM throws `DeviceInfoPlusException` (`NotSupported`) so tests inject `IDeviceProbe`.
- Capability flags mean hardware presence, not permission, radio-on, or biometric enrollment.
- iPhone GPS is treated as present. iPad GPS is true only when a WWAN interface exists (cellular models).
- This is not a unique device identifier. Use Plugin.Maui.DeviceSession for install/device ids.
- Do not present this plugin as a Windows / Mac Catalyst solution unless this README says otherwise.
