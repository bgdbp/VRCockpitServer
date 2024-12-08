#include <Adafruit_NeoPixel.h>

Adafruit_NeoPixel strip(16, 6, NEO_GRB + NEO_KHZ800);

void setup() {
  // put your setup code here, to run once:
  Serial.begin(9600);
  strip.begin();
  strip.show();
  strip.setBrightness(25);

}

void loop() {
  // put your main code here, to run repeatedly:
  if (Serial.available() > 0)
  {
    strip.clear();
    char incomingByte = Serial.read();

    //incomingByte -= '0';
    incomingByte = 15 - incomingByte;

    if (incomingByte > 15 or incomingByte < 0)
    {
      return;
    }

    strip.setPixelColor(incomingByte, strip.Color(0, 0, 180));
    for (int i = 15; i > incomingByte; i--)
      strip.setPixelColor(i, strip.Color(50, 50, 50));

    strip.show();
  }
}
