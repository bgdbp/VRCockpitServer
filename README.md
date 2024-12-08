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

This server does not have proper user authentication or encryption, so it is advised
to only run this server on a secure private network. 


