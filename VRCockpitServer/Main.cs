using System.Net;
using System.Net.Sockets;
using VRCockpitServer;

string hostName = Dns.GetHostName();
IPHostEntry localhost = Dns.GetHostEntry(hostName);
IPAddress localhostIP = localhost.AddressList[0];
IPEndPoint ipEndPoint = new (localhostIP, 11000);

using Socket listener = new (ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

listener.Bind(ipEndPoint);
listener.Listen(100);

while (true)
{
    try
    {
        Console.WriteLine("Listening for new connection...");
        Socket connection = await listener.AcceptAsync();
        ConnectionHandler conHandler = new (connection);

        var ipEndpoint = (IPEndPoint?)(connection.RemoteEndPoint);
        if (ipEndpoint != null)
        {
            Console.WriteLine($"Connected to {ipEndpoint} --- Receiving!");
            _ = conHandler.Receive();
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Connection error: {ex}");
    }
}
