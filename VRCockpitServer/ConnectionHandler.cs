using System.Text;
using System.Net.Sockets;
using System.Text.Json;
using VRCockpitServer.CommClasses;

namespace VRCockpitServer
{
    internal class ConnectionHandler
    {
        readonly Socket connection;
        const int BUFFER_SIZE = 2048;
        readonly byte[] buffer = new byte[BUFFER_SIZE];
        readonly byte[] messageBuffer = new byte[BUFFER_SIZE * 8];

        public ConnectionHandler(Socket connection)
        {
            this.connection = connection;
        }

        public async Task Receive()
        {
            try
            {
                int messageBufferIndex = 0;
                int messageSize = 0;

                while (true)
                {
                    // read message from socket into byte buffer
                    while (messageSize == 0 || messageBufferIndex < messageSize)
                    {
                        int bytesReceived = await connection.ReceiveAsync(buffer);

                        if (bytesReceived == 0)
                        {
                            Console.WriteLine("Connection closed.");
                            connection.Close();
                            return;
                        }

                        Buffer.BlockCopy(buffer, 0, messageBuffer, messageBufferIndex, bytesReceived);

                        // obtain the size of the message, stored at the front
                        if (messageSize == 0)
                        {
                            messageSize = BitConverter.ToInt32(messageBuffer, startIndex: 0);
                        }

                        messageBufferIndex += bytesReceived;
                    }

                    // obtain JSON string containing the request object from the buffer
                    //string requestStr = Encoding.UTF8.GetString(messageBuffer, index: sizeof(int), messageSize - sizeof(int));

                    // Deserialize the incoming JSON request object
                    //VRCRequest? incomingRequest = JsonSerializer.Deserialize<VRCRequest>(requestStr);
                    VRCRequest? incomingRequest = await JsonSerializer.DeserializeAsync<VRCRequest>(new MemoryStream(messageBuffer, index: sizeof(int), messageSize - sizeof(int)));

                    // Handle the request.
                    if (incomingRequest != null)
                    {
                        await incomingRequest.HandleRequest();
                    }

                    // send bytes of the next messages to the front of the buffer
                    byte[] nextMessageBytes = messageBuffer[messageSize..messageBufferIndex];
                    nextMessageBytes.CopyTo(messageBuffer, 0);

                    messageBufferIndex = messageBufferIndex - messageSize;

                    if (messageBufferIndex >= sizeof(int))
                    {
                        messageSize = BitConverter.ToInt32(messageBuffer, startIndex: 0);
                    }
                    else
                    {
                        messageSize = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
            }
        }
    }
}
