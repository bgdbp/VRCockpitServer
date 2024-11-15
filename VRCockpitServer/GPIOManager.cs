using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Device.Pwm.Drivers;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRCockpitServer
{
    internal class GPIOManager : IDisposable
    {
        public static GPIOManager? Instance;

        public static void Init()
        {
            Instance = new GPIOManager();
        }

        Dictionary<string, int> pinMap = new()
        {
            {"Knob", 4}
        };

        GpioController controller;
        SoftwarePwmChannel knobPwm;

        public GPIOManager()
        {
            try
            {
                controller = new GpioController();
                knobPwm = new SoftwarePwmChannel(pinNumber: 4, controller: controller, dutyCycle: 0);
                knobPwm.Start();
            }
            catch (Exception ex)
            {
                controller = null;
                Console.WriteLine("GPIO not supported on this platform");
            }
        }

        public void Dispose()
        {
            controller?.Dispose();
            knobPwm.Stop();
            knobPwm?.Dispose();
        }
        public static void SetPwmPin(int pinNumber, float value)
        {
            if (Instance == null)
                return;

            if (Instance.controller == null)
                return;

            if (value > 1 || value < 0)
                return;

            Instance.knobPwm.DutyCycle = value;
        }

        public void SetPin(int pinNumber, bool value)
        {
            if (controller == null)
                return;

            controller.Write(pinNumber, (value) ? PinValue.High : PinValue.Low);
        }
    }
}
