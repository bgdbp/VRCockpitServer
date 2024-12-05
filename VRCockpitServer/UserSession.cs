using System.Text;
using System.Net.Sockets;
using System.Text.Json;
using VRCockpitServer.CommClasses;
using System.Net.Http.Headers;
using System.Diagnostics;

namespace VRCockpitServer
{
    internal class UserSession
    {
        static HashSet<UserSession> users = [];
        readonly TcpClient connection;

        public UserSession(TcpClient connection)
        {
            this.connection = connection;
            byte[] bytes = Encoding.UTF8.GetBytes("[");
            connection.GetStream().Write(bytes, 0, bytes.Length);
        }

        public static void BroadcastRequest(VRCRequest request, UserSession? ignoredUser = null)
        {
            foreach (UserSession user in users)
            {
                if (user == null) continue;
                if (user == ignoredUser) continue;


                user.SendRequest(request);
            }
        }

        public async void SendRequest(VRCRequest request)
        {
            while (this != null)
            {
                try
                {
                    var networkStream = connection.GetStream();
                    string message = JsonSerializer.Serialize(request);
                    message += ",";

                    byte[] bytes = Encoding.UTF8.GetBytes(message);
                    await networkStream.WriteAsync(bytes, 0, bytes.Length);
                    return;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        public async Task Receive()
        {
            try
            {
                users.Add(this);
                var stream = connection.GetStream();
                JsonSerializerOptions options = new JsonSerializerOptions() { AllowTrailingCommas = true };
                var incomingRequests = JsonSerializer.DeserializeAsyncEnumerable<VRCRequest>(stream, options);

                await foreach (var incomingRequest in incomingRequests)
                {
                    if (incomingRequest != null)
                    {
                        await incomingRequest.HandleRequest(this);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
            }
            finally
            {
                string message = "]";

                byte[] bytes = Encoding.UTF8.GetBytes(message);
                await connection.GetStream().WriteAsync(bytes, 0, bytes.Length);

                connection.LingerState = new LingerOption(true, 0);
                connection.Close();
                users.Remove(this);
                Console.WriteLine($"Connection closed.");
            }
        }
    }
}
