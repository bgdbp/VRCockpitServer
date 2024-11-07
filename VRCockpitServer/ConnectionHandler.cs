using System.Text;
using System.Net.Sockets;
using System.Text.Json;
using VRCockpitServer.CommClasses;
using System.Net.Http.Headers;

namespace VRCockpitServer
{
    internal class ConnectionHandler
    {
        readonly TcpClient connection;

        public ConnectionHandler(TcpClient connection)
        {
            this.connection = connection;
        }

        public async Task Receive()
        {
            try
            {
                var stream = connection.GetStream();
                JsonSerializerOptions options = new JsonSerializerOptions() { AllowTrailingCommas = true };
                var incomingRequests = JsonSerializer.DeserializeAsyncEnumerable<VRCRequest>(stream, options);

                await foreach (var incomingRequest in incomingRequests)
                {
                    if (incomingRequest != null)
                    {
                        await incomingRequest.HandleRequest();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
            }
            finally
            {
                connection.Close();
                Console.WriteLine($"Connection closed.");
            }
        }
    }
}
