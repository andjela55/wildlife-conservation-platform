# ESP8266/ESP32 collar simulator

This PlatformIO project targets an ESP8266 NodeMCU (`nodemcuv2`) or a common
ESP32 DevKit (`esp32dev`). It sends the next point in a small simulated route
every 10 seconds.

## Configure

Edit `include/secrets.h` with the local credentials. This file is ignored by
Git; `include/secrets.example.h` is the safe template to commit:

- Set `WIFI_SSID` and `WIFI_PASSWORD` to a 2.4 GHz WiFi network.
- Set `DEVICE_API_KEY` to the same value configured on the backend. Keep
  `JWT_TOKEN` empty when using the device key.

Edit the non-secret constants at the top of `src/main.cpp`:

- Set `API_URL` to the backend's HTTP URL using the laptop's LAN address, such
  as `http://192.168.1.25:5191/api/location-points`. Do not use `localhost`.
- Set `ANIMAL_ID` and `COLLAR_ID` to records that exist in the database and
  belong together.

Configure the backend key without committing it:

```powershell
cd backend/WildlifeConservation.Api
dotnet user-secrets set "DeviceApiKey" "choose-a-long-random-local-development-key"
```

For access from the ESP, bind the API to all laptop network interfaces. The
port must match `API_URL`:

```powershell
dotnet run --urls http://0.0.0.0:5191
```

Allow inbound TCP port 5191 through the laptop firewall if necessary. The ESP
and laptop must be reachable on the same LAN.

## Upload and monitor

Connect the NodeMCU by USB, then run from this directory:

```powershell
pio run --environment nodemcuv2 --target upload
pio device monitor --environment nodemcuv2
```

For a common ESP32 DevKit, replace `nodemcuv2` with `esp32dev`. In the
PlatformIO toolbar, select the matching environment before Build or Upload.

The monitor runs at 115200 baud and prints the WiFi result, JSON payload, HTTP
status, and backend response. A successful POST returns HTTP 201.
