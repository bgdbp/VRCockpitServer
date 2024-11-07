using System.Net;
using System.Net.Sockets;
using VRCockpitServer;

string hostName = Dns.GetHostName();
IPHostEntry localhost = Dns.GetHostEntry(hostName);
IPAddress localhostIP = localhost.AddressList[0];
IPEndPoint ipEndPoint = new (localhostIP, 11004);

//using Socket listener = new (ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
using TcpListener listener = new(ipEndPoint);
listener.Start();

while (true)
{
    try
    {
        Console.WriteLine("Listening for new connection...");
        TcpClient client = await listener.AcceptTcpClientAsync();

        ConnectionHandler conHandler = new (client);

        var ipEndpoint = (IPEndPoint?)(client.Client.RemoteEndPoint);
        if (ipEndpoint != null)
        {
            Console.WriteLine($"Connected to {ipEndpoint} --- Receiving!");
            Task task = conHandler.Receive();
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Connection error: {ex}");
    }
}
