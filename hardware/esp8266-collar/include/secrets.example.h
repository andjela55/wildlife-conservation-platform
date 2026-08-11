#pragma once

// Copy this file to secrets.h and enter local values. Never commit secrets.h.
constexpr char WIFI_SSID[] = "YOUR_WIFI_SSID";
constexpr char WIFI_PASSWORD[] = "YOUR_WIFI_PASSWORD";

// Preferred device authentication. Must match the backend DeviceApiKey value.
constexpr char DEVICE_API_KEY[] = "YOUR_DEVICE_API_KEY";

// Optional local fallback. Leave empty when DEVICE_API_KEY is configured.
constexpr char JWT_TOKEN[] = "";
