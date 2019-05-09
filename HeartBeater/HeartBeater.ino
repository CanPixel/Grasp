#include <Wire.h>
#include <SoftwareSerial.h>

#define joyX A0
#define joyY A1
#define joyButton 2

int xVal, yVal, xMap, yMap;

const float joyStickSmoothness = 100;

unsigned char reading;

void setup() {
  Serial.begin(115200);

  pinMode(joyX, INPUT_PULLUP);
  pinMode(joyY, INPUT_PULLUP);
  pinMode(joyButton, INPUT_PULLUP);
  
  Wire.begin();
}

void loop() {
  Wire.requestFrom(0xA0 >> 1, 1, true);
  while(Wire.available()) {
    reading = Wire.read();
  }
  xVal = analogRead(joyX);
  yVal = analogRead(joyY);
  xMap = map(xVal, 0, 1023, -joyStickSmoothness - 2, joyStickSmoothness);
  yMap = map(yVal, 0, 1023, -joyStickSmoothness - 2, joyStickSmoothness);

  String controls = String(reading) + "|" + String(xMap) + "," + String(yMap) + ":" + String(digitalRead(joyButton));
  Serial.println(controls);
  Serial.flush();
  delay(42);
}
