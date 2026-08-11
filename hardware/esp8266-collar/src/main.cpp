#include <Arduino.h>

#if defined(ESP8266)
#include <ESP8266HTTPClient.h>
#include <ESP8266WiFi.h>
#elif defined(ESP32)
#include <HTTPClient.h>
#include <WiFi.h>
#else
#error "This collar simulator supports ESP8266 and ESP32 boards."
#endif

#include <WiFiClient.h>
#include "secrets.h"

// Update these non-secret values before uploading. API_URL must use the
// laptop's LAN IP, never localhost or 127.0.0.1.
const char *API_URL = "http://192.168.100.13:5191/api/location-points";
const int ANIMAL_ID = 1;
const int COLLAR_ID = 1;

namespace
{
struct RoutePoint
{
  double latitude;
  double longitude;
};

const RoutePoint ROUTE[] = {
    {-3.119000, -60.021700},
    {-3.118760, -60.021410},
    {-3.118480, -60.021080},
    {-3.118190, -60.020790},
    {-3.118450, -60.020470},
    {-3.118790, -60.020820},
    {-3.119090, -60.021190},
};

const size_t ROUTE_POINT_COUNT = sizeof(ROUTE) / sizeof(ROUTE[0]);
const unsigned long SEND_INTERVAL_MS = 10000;

size_t routeIndex = 0;
unsigned long lastSendAt = 0;

void connectToWiFi()
{
  if (WiFi.status() == WL_CONNECTED)
  {
    return;
  }

  Serial.printf("Connecting to WiFi '%s'", WIFI_SSID);
  WiFi.mode(WIFI_STA);
  WiFi.begin(WIFI_SSID, WIFI_PASSWORD);

  while (WiFi.status() != WL_CONNECTED)
  {
    delay(500);
    Serial.print('.');
  }

  Serial.println();
  Serial.print("WiFi connected. ESP IP address: ");
  Serial.println(WiFi.localIP());
}

String createPayload(const RoutePoint &point)
{
  String payload;
  payload.reserve(220);
  payload += F("{\"animalId\":");
  payload += ANIMAL_ID;
  payload += F(",\"collarId\":");
  payload += COLLAR_ID;
  payload += F(",\"latitude\":");
  payload += String(point.latitude, 6);
  payload += F(",\"longitude\":");
  payload += String(point.longitude, 6);
  payload += F(",\"altitude\":null");
  payload += F(",\"signalType\":\"Simulator\"");
  payload += F(",\"notes\":\"ESP8266 simulated collar signal\"}");
  return payload;
}

void sendLocationPoint()
{
  connectToWiFi();

  const String payload = createPayload(ROUTE[routeIndex]);
  WiFiClient client;
  HTTPClient http;

  Serial.print("POST ");
  Serial.println(API_URL);
  Serial.print("Payload: ");
  Serial.println(payload);

  if (!http.begin(client, API_URL))
  {
    Serial.println("Unable to start HTTP request");
    return;
  }

  http.addHeader("Content-Type", "application/json");
  if (strlen(DEVICE_API_KEY) > 0)
  {
    http.addHeader("X-Device-Key", DEVICE_API_KEY);
  }
  else if (strlen(JWT_TOKEN) > 0)
  {
    http.addHeader("Authorization", String("Bearer ") + JWT_TOKEN);
  }

  const int statusCode = http.POST(payload);
  Serial.print("HTTP status: ");
  Serial.println(statusCode);

  if (statusCode > 0)
  {
    Serial.print("Response: ");
    Serial.println(http.getString());
  }
  else
  {
    Serial.print("Request error: ");
    Serial.println(http.errorToString(statusCode));
  }

  http.end();
  routeIndex = (routeIndex + 1) % ROUTE_POINT_COUNT;
}
} // namespace

void setup()
{
  Serial.begin(115200);
  delay(1500);
  Serial.println();
  Serial.println("Wildlife collar simulator starting");
  connectToWiFi();
  sendLocationPoint();
  lastSendAt = millis();
}

void loop()
{
  const unsigned long now = millis();
  if (now - lastSendAt >= SEND_INTERVAL_MS)
  {
    lastSendAt = now;
    sendLocationPoint();
  }

  delay(10);
}
