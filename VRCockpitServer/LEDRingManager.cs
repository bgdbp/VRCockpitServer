using System.IO.Ports;

namespace VRCockpitServer
{
    internal class LEDRingManager : IDisposable
    {
        public static LEDRingManager Instance = null!;

        SerialPort ledRingPort;

        public LEDRingManager()
        {
            ledRingPort = new SerialPort("/dev/ttyACM0", 9600);
        }

        async Task initialize()
        {
            ledRingPort.Open();

            Console.WriteLine("Waiting for Arduino to finish booting...");
            await Task.Delay(2000);

            byte[] n = { 0 };

            Console.WriteLine("Writing byte to Arduino...");
            ledRingPort.Write(n, 0, 1);
            Console.WriteLine("Byte written");
        }

        public static async Task<LEDRingManager> Init()
        {
            Instance = new LEDRingManager();
            await Instance.initialize();
            return Instance;
        }

        public void SetKnobValue(byte value)
        {
            byte[] n = { value };

            try
            {
                ledRingPort.Write(n, 0, 1);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Cannot write to LED ring, it is not connected ({ex.Message})");
            }
        }

        public void Dispose()
        {
            ledRingPort.Close();
        }
    }
}