#include <Wire.h>
#include <SoftwareSerial.h>

void setup() {
  Serial.begin(115200);
  Serial.println("heart rate sensor:");
  Wire.begin();
  while(!Serial);
}
void loop() {
  Wire.requestFrom(0xA0 >> 1, 1);
  while(Wire.available()) {
    unsigned char c = Wire.read();
    Serial.println(c, DEC);
    Serial.flush();
  }
  delay(42);
}
