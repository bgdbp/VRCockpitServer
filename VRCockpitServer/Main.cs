using System.Net;
using System.Net.Sockets;
using System.Text;
using VRCockpitServer;
using System.IO.Ports;
using VRCockpitServer.CommClasses;

async Task Main()
{
    /*
    try
    {
    }
    catch (Exception ex)
    {
        Console.WriteLine($"LEDRingManager could not be initialized ({ex.Message})");
    }
    */

    using LEDRingManager ledRingManager = await LEDRingManager.Init();
    using GPIOManager gpioManager = GPIOManager.Init();

    RequestVRCToggle rToggle = new() { ControlID = "Lights"};
    rToggle.IsOn = false;
    await rToggle.HandleRequest(null);

    RequestVRCButton rButton = new() { ControlID = "RedButton"};
    rButton.IsPressed = false;
    await rButton.HandleRequest(null);

    RequestVRCKnob rKnob = new() { ControlID = "Intensity"};
    rKnob.Value = 0;
    await rKnob.HandleRequest(null);

    rToggle.IsOn = false;

    string hostName = Dns.GetHostName();
    IPHostEntry localhost = Dns.GetHostEntry(hostName);
    IPAddress localhostIP = localhost.AddressList.First(x => x.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(x));

    foreach (var addr in localhost.AddressList)
    {
        if (localhostIP == addr)
        {
            Console.WriteLine(addr.ToString() + " <--- CHOSEN");
        }
        else
        {
            Console.WriteLine(addr.ToString());
        }
    }

    IPEndPoint tcpEndPoint = new (localhostIP, 11004);
    IPEndPoint udpEndPoint = new (IPAddress.Any, 11000);

    using TcpListener listener = new(tcpEndPoint);
    using UdpClient discoveryClient = new(udpEndPoint);

    listener.Start();

    List<Task> serverTasks = [receiveAndHandleUDP(), receiveAndHandleTCP()];
    await Task.WhenAll(serverTasks);

    async Task receiveAndHandleUDP()
    {
        while (true)
        {
            var message = await discoveryClient.ReceiveAsync();
            string messageContents = Encoding.UTF8.GetString(message.Buffer);

            if (messageContents == "VRCDiscover")
            {
                string tcpEndPointMessage = $"VRCDiscover|{tcpEndPoint.Address}|{tcpEndPoint.Port}";
                byte[] tcpEndPointMessageBytes = Encoding.UTF8.GetBytes(tcpEndPointMessage);
                await discoveryClient.SendAsync(tcpEndPointMessageBytes, tcpEndPointMessageBytes.Length, message.RemoteEndPoint);
            }
        }
    }
    async Task receiveAndHandleTCP()
    {
        while (true)
        {
            try
            {
                Console.WriteLine("Listening for new connection...");
                TcpClient client = await listener.AcceptTcpClientAsync();

                UserSession user = new (client);

                var ipEndpoint = (IPEndPoint?)(client.Client.RemoteEndPoint);
                if (ipEndpoint != null)
                {
                    Console.WriteLine($"Connected to {ipEndpoint} --- Receiving!");
                    Task task = user.Receive();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Connection error: {ex}");
            }
        }
    }
}

try
{
    await Main();
}
catch (Exception ex)
{
    Console.WriteLine(ex.ToString());
}
