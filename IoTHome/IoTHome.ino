#include <ESP8266WiFi.h>
#include <WiFiClient.h>
#include <WiFiServerSecureBearSSL.h>
#include <ArduinoJson.h>
#include <LittleFS.h>
#include <TZ.h>

const char* ssid = "ASUS_Top";
const char* password = "keVu4?SW";

IPAddress ip(192,168,3,34);
IPAddress gateway(192,168,3,1);
IPAddress subnet(255,255,255,0);
#define PORT 40069

BearSSL::ServerSessions serverCache(5);
BearSSL::WiFiServerSecure server(PORT);

const int LED = 2;

namespace {
  class Capability
  {
  public:
    Capability(const String& name, const String& description, const String& url, const String& type)
      : m_name(name)
      , m_description(description)
      , m_url(url)
      , m_type(type)
    {
    }
  
    const String& GetName() const { return m_name; }
    const String& GetDescription() const { return m_description; }
    const String& GetUrl() const { return m_url; }
    const String& GetType() const { return m_type; }
  
  private:
    String m_name;
    String m_description;
    String m_url;
    String m_type;
  };
  
  bool convertToJson(const Capability& src, JsonVariant dst) {
    dst["name"] = src.GetName();
    dst["description"] = src.GetDescription();
    dst["url"] = src.GetUrl();
    dst["type"] = src.GetType();
    
    return true;
  }
}

namespace {
  class Slider
  {
  public:
    Slider() {}
    
    Slider(int min, int max, int current)
      : m_min(min)
      , m_max(max)
    {
      if (m_min > m_max)
      {
        int temp = m_min;
        m_min = m_max;
        m_max = temp;
      }
     SetCurrent(current);
    }
  
    void SetCurrent(int current)
    {
      m_current = current;
      if (m_current < m_min)
        m_current = m_min;
      if (m_current > m_max)
        m_current = m_max;
    }
  
    int GetCurrent() const
    {
      return m_current;
    }
  
    int GetMin() const
    {
      return m_min;
    }
  
    int GetMax() const
    {
      return m_max;
    }
  
  private:
    int m_min;
    int m_max;
    int m_current;
  };
  
  bool convertToJson(const Slider& src, JsonVariant dst) {
    dst["min"] = src.GetMin();
    dst["max"] = src.GetMax();
    dst["current"] = src.GetCurrent();
    return true;
  }
  
  void convertFromJson(JsonVariantConst src, Slider& dst) {
    int min = src["min"];
    int max = src["max"];
    int current = src["current"];
  
    dst = Slider(min, max, current);
  }
  
  bool canConvertFromJson(JsonVariantConst src, const Slider&) {
    return src["min"].is<double>() && src["max"].is<double>() && src["current"].is<double>();
  }
}


namespace {
  class Toggle
  {
  public:
    Toggle() {}
    Toggle(bool state) : m_state(state)
    {}
  
    bool IsToggled() const
    {
      return m_state;
    }
  
    void SetToggle(bool state)
    {
      m_state = state;
    }
  
  private:
    bool m_state;
  };
  
  bool convertToJson(const Toggle& src, JsonVariant dst) {
    dst["isToggled"] = src.IsToggled();
    return true;
  }
  
  void convertFromJson(JsonVariantConst src, Toggle& dst) {
    bool isToggled = src["isToggled"];
    dst = Toggle(isToggled);
  }
  
  bool canConvertFromJson(JsonVariantConst src, const Toggle&) {
    return src["isToggled"].is<bool>();
  }
}


bool ReadFromFile(const String& path, String& out)
{
  File file = LittleFS.open(path, "r");
  if (!file)
    return false;

  out = file.readString();
  return true;
}




Slider slider(10, 100, 40);
Toggle toggle;



String HandleMessage(const String& message, const String& data, String& response)
{
  if (message == "DeviceCapabilities")
  {
    Capability capabilities[2] = { 
      Capability("Test slider", "", "slider", "slider"), 
      Capability("LED toggle switch", "", "toggleButton", "toggleButton") 
    };

    const size_t CAPACITY = JSON_ARRAY_SIZE(2) * JSON_OBJECT_SIZE(4);
    StaticJsonDocument<CAPACITY> doc;
    JsonArray array = doc.to<JsonArray>();
    for (int i = 0; i < 2; i++)
    {
      array.add(capabilities[i]);
    }
    String capabilitiesStr;
    serializeJson(doc, capabilitiesStr);
    response = message + capabilitiesStr;
    return "";
  }
  if (message == "DeviceCapability")
  {
    String type;
    {
      const size_t CAPACITY = JSON_OBJECT_SIZE(1);
      StaticJsonDocument<CAPACITY> doc;
      DeserializationError error = deserializeJson(doc, data);
      if (error)
        return error.f_str();
      type = doc.as<String>();
    }

    const size_t CAPACITY = JSON_OBJECT_SIZE(4);
    StaticJsonDocument<CAPACITY> doc;
    if (type == "slider")
      doc.set(slider);
    else if (type == "toggleButton")
      doc.set(toggle);
    else
      return "Unknown capability: " + type;
      
    String capabilityStr;
    serializeJson(doc, capabilityStr);
    
    response = message + capabilityStr;
    return "";
  }
  if (message == "slider")
  {
    {
      const size_t CAPACITY = JSON_OBJECT_SIZE(4);
      StaticJsonDocument<CAPACITY> doc;
      DeserializationError error = deserializeJson(doc, data);
      if (error)
        return error.f_str();
  
      if (!doc.is<Slider>())
        return "Cannot extract slider from data: " + data;
  
      Slider s = doc.as<Slider>();
      if (s.GetMin() != slider.GetMin() || s.GetMax() != slider.GetMax())
        return "Cannot change boundaries of slider";
  
      slider.SetCurrent(s.GetCurrent());
      Serial.println(slider.GetCurrent());
    }
    const size_t CAPACITY = JSON_OBJECT_SIZE(4);
    StaticJsonDocument<CAPACITY> doc;
    doc.set(slider);
    String modelStr;
    serializeJson(doc, modelStr);
    response = message + modelStr;
    return "";
  }
  if (message == "toggleButton")
  {
    {
      const size_t CAPACITY = JSON_OBJECT_SIZE(4);
      StaticJsonDocument<CAPACITY> doc;
      DeserializationError error = deserializeJson(doc, data);
      if (error)
        return error.f_str();
  
      if (!doc.is<Toggle>())
        return "Cannot extract toggleButton from data: " + data;
  
      Toggle t = doc.as<Toggle>();
  
      toggle.SetToggle(t.IsToggled());
      digitalWrite(LED, toggle.IsToggled());
    }
    const size_t CAPACITY = JSON_OBJECT_SIZE(4);
    StaticJsonDocument<CAPACITY> doc;
    doc.set(toggle);
    String modelStr;
    serializeJson(doc, modelStr);
    response = message + modelStr;
    return "";
  }
  return "Unknown message";
}

void setup(void) {
  pinMode(LED, OUTPUT);
  digitalWrite(LED, 0);
  
  Serial.begin(115200);
  WiFi.mode(WIFI_STA);
  WiFi.config(ip, gateway, subnet, gateway);
  WiFi.begin(ssid, password);
  Serial.println("");

  LittleFS.begin();

  // Wait for connection
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }
  Serial.println("");
  Serial.print("Connected to ");
  Serial.println(ssid);
  Serial.print("IP address: ");
  Serial.println(WiFi.localIP());

  configTime(TZ_Europe_Kiev, "pool.ntp.org");
  
  while (time(nullptr) < 1662317449) // some time as on 04.09.2022
  {
    Serial.print("*");
    delay(500);
  }
  Serial.println("");
  Serial.println("Time has been synced");

  server.setCache(&serverCache);
  
  String cert;
  String key;
  if (!ReadFromFile("iot1.crt", cert))
  {
    Serial.println("Certificate is missing");
    return;
  }
  
  if (!ReadFromFile("iot1.key", key))
  {
    Serial.println("Key is missing");
    return;
  }
  
  server.setRSACert(new BearSSL::X509List(cert.c_str()), new BearSSL::PrivateKey(key.c_str()));
  server.begin();
}

void loop(void) {
  BearSSL::WiFiClientSecure client = server.available();
  if (client)
  {
    Serial.println("connecting");
    while (client.connected()) {
      if (client.available() == 0)
        continue;
      String message;
      String data;
      if (client.available() > 0){
        Serial.println("reading");
        message = client.readStringUntil('\n');
        Serial.println(message);
      }
      
      if (client.available() > 0){
        data = client.readStringUntil('\n');
        Serial.println(data);
      }
      
      String response;
      String error = HandleMessage(message, data, response);
      if (error.length() > 0)
      {
        Serial.println(error);
        client.println("error" + error);
      } 
      else 
      {
        Serial.println(response);
        client.println(response);
      }
    }
    Serial.println("disconnecting");
    client.stop();
  } 
}
