using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Device.Pwm.Drivers;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRCockpitServer.CommClasses;

namespace VRCockpitServer
{
    internal class GPIOManager : IDisposable
    {
        public const int JOYSTICK_INPUT_UP = 5;
        public const int JOYSTICK_INPUT_DOWN = 6;

        public const int JOYSTICK_OUTPUT_UP = 16;
        public const int JOYSTICK_OUTPUT_DOWN = 26;

        public const int KNOB_INPUT_CLOCKWISE = 23;
        public const int KNOB_INPUT_COUNTERCLOCKWISE = 24;

        public const int BUTTON_INPUT = 17;
        public const int BUTTON_OUTPUT = 27;

        public static GPIOManager? Instance;


        public static GPIOManager Init()
        {
            Instance = new GPIOManager();
            return Instance;
        }

        GpioController controller;

        public GPIOManager()
        {
            try
            {
                controller = new GpioController();


                //joystick input
                controller.OpenPin(JOYSTICK_INPUT_UP, PinMode.InputPullUp);
                controller.RegisterCallbackForPinValueChangedEvent(JOYSTICK_INPUT_UP, PinEventTypes.Falling, (o, e) => { HandleToggleOn(); });

                controller.OpenPin(JOYSTICK_INPUT_DOWN, PinMode.InputPullUp);
                controller.RegisterCallbackForPinValueChangedEvent(JOYSTICK_INPUT_DOWN, PinEventTypes.Falling, (o, e) => { HandleToggleOff(); });

                //joystick output
                controller.OpenPin(JOYSTICK_OUTPUT_UP, PinMode.Output); //on
                controller.OpenPin(JOYSTICK_OUTPUT_DOWN, PinMode.Output); //off

                //joystick initialize
                controller.Write(26, PinValue.High); //initialize toggle to off position

                //knob input (knob output is handled in LEDRingManager)
                controller.OpenPin(KNOB_INPUT_CLOCKWISE, PinMode.InputPullUp);
                controller.RegisterCallbackForPinValueChangedEvent(KNOB_INPUT_CLOCKWISE, PinEventTypes.Falling, (o, e) => { HandleKnobClockwise(); });

                controller.OpenPin(KNOB_INPUT_COUNTERCLOCKWISE, PinMode.InputPullUp);
                controller.RegisterCallbackForPinValueChangedEvent(KNOB_INPUT_COUNTERCLOCKWISE, PinEventTypes.Falling, (o, e) => { HandleKnobCounterClockwise(); });

                //button input
                controller.OpenPin(BUTTON_INPUT, PinMode.InputPullUp);
                controller.RegisterCallbackForPinValueChangedEvent(BUTTON_INPUT, PinEventTypes.Falling, (o, e) => { SetButton(isPressed: true); });
                controller.RegisterCallbackForPinValueChangedEvent(BUTTON_INPUT, PinEventTypes.Rising, (o, e) => { SetButton(isPressed: false); });

                //button output
                controller.OpenPin(BUTTON_OUTPUT, PinMode.Output);

            }
            catch (Exception ex)
            {
                controller = null;
                Console.WriteLine("GPIO not supported on this platform");
            }
        }


        public void HandleToggleOn()
        {
        }

        public void HandleToggleOff()
        {

        }

        public void HandleKnobClockwise()
        {

        }

        public void HandleKnobCounterClockwise()
        {

        }

        public void SetButton(bool isPressed)
        {
            RequestVRCButton? rButton = RequestVRCControl.GetControlByID("RedButton") as RequestVRCButton;

            if (rButton == null)
                return;

            rButton.IsPressed = isPressed;

            rButton.HandleRequest(null);
        }


        public void Dispose()
        {
            controller?.Dispose();
        }

        private static void SetPin(int pinNumber, bool value)
        {
            if (Instance == null)
                return;

            if (Instance.controller == null)
                return;

            Instance.controller.Write(pinNumber, (value) ? PinValue.High : PinValue.Low);
        }
    }
}
