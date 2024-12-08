**VR Cockpit Server v1.0.0**

This is the server side of our VR Cockpit project, it is responsible for
accepting connections from VR Cockpit clients, synchronizing their cockpit controls
between the server and client, displaying the control states on the physical
cockpit panel, as well as taking input from the physical controls to be sent
to the client. The server should be installed onto a Raspberry Pi 5. 

The VR Cockpit client
is built directly to the VR Headset, its repository is located here: https://github.com/bgdbp/VRCockpit

**Build Instructions**

To build this server, clone this repository onto a Windows machine and open the Visual Studio
solution file at the root of this repository. We used Visual Studio Community 2022 for this project.
Under the Build tab, click Publish Selection,
and then click Publish. Copy the resulting build folder, called linux-arm64 onto the Raspberry Pi and give the
VRCockpitServer program execute permissions by running the following command in the linux-arm64 folder: ```chmod +x VRCockpitServer```

To have the server boot when the Pi is powered on, create
a systemd service for the VRCockpitServer program, this is optional. 

The pin mapping for the physical controls and LED control state indicators is
located in the top of GPIOManager.cs

For the Adafruit Neopixel 16 count LED Ring to function, it utilizes an Arduino Uno R3. Open the LEDRing.ino
file inside of the LEDRing folder in Arduino IDE. Make sure you have the Adafruit
NeoPixel library installed. To do this, navigate to Tools->Manage Libraries, 
search Adafruit NeoPixel, and install. Make sure the Arduino is connected to your
computer, you may need to navigate to Tools->Com and select your device there.
Click the right arrow at the top left of Arduino IDE to upload this code to your Arduino.
Finally, connect your Arduino to the Pi using this USB cable, make sure to connect
to the top right USB port of the Pi. 

The LED Ring must be connected to the Arduino by 5v, ground, and
pin 6 for the data. 

This server does not have proper user authentication or encryption, so it is advised
to only run this server on a secure private network. 


