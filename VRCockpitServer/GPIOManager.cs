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

        public const int KNOB_INPUT_CLOCKWISE = 23;
        public const int KNOB_INPUT_COUNTERCLOCKWISE = 24;

        public const int BUTTON_INPUT = 17;

        public const int BUTTON_OUTPUT = 4;
        public const int JOYSTICK_OUTPUT_UP = 16;
        public const int JOYSTICK_OUTPUT_DOWN = 26;

        public static GPIOManager? Instance;


        public static GPIOManager Init()
        {
            Instance = new GPIOManager();
            return Instance;
        }

        GpioController controller = null!;

        public GPIOManager()
        {
            try
            {
                controller = new GpioController();


                //joystick input
                controller.OpenPin(JOYSTICK_INPUT_UP, PinMode.InputPullUp);
                controller.RegisterCallbackForPinValueChangedEvent(JOYSTICK_INPUT_UP, PinEventTypes.Falling, (o, e) => HandleToggleChange(isOn: true));

                controller.OpenPin(JOYSTICK_INPUT_DOWN, PinMode.InputPullUp);
                controller.RegisterCallbackForPinValueChangedEvent(JOYSTICK_INPUT_DOWN, PinEventTypes.Falling, (o, e) => HandleToggleChange(isOn: false));

                //joystick output
                controller.OpenPin(JOYSTICK_OUTPUT_UP, PinMode.Output); 
                controller.OpenPin(JOYSTICK_OUTPUT_DOWN, PinMode.Output); 

                //joystick initialize
                controller.Write(JOYSTICK_OUTPUT_UP, PinValue.Low);
                controller.Write(JOYSTICK_OUTPUT_DOWN, PinValue.Low);

                //knob input (knob output is handled in LEDRingManager)
                controller.OpenPin(KNOB_INPUT_CLOCKWISE, PinMode.InputPullUp);
                controller.RegisterCallbackForPinValueChangedEvent(KNOB_INPUT_CLOCKWISE, PinEventTypes.Rising, (o, e) => { HandleKnobChange(isClockwise: true); });

                controller.OpenPin(KNOB_INPUT_COUNTERCLOCKWISE, PinMode.InputPullUp);
                controller.RegisterCallbackForPinValueChangedEvent(KNOB_INPUT_COUNTERCLOCKWISE, PinEventTypes.Rising, (o, e) => { HandleKnobChange(isClockwise: false); });

                //button input
                controller.OpenPin(BUTTON_INPUT, PinMode.InputPullUp);
                controller.RegisterCallbackForPinValueChangedEvent(BUTTON_INPUT, PinEventTypes.Falling, (o, e) => { HandleButtonChange(isPressed: true); });
                controller.RegisterCallbackForPinValueChangedEvent(BUTTON_INPUT, PinEventTypes.Rising, (o, e) => { HandleButtonChange(isPressed: false); });

                //button output
                controller.OpenPin(BUTTON_OUTPUT, PinMode.Output);
                controller.Write(BUTTON_OUTPUT, PinValue.Low);

                Console.WriteLine($"Toggle up pin mode: {controller.GetPinMode(JOYSTICK_INPUT_UP)}");
                //Console.WriteLine($"Toggle down pin mode: {controller.GetPinMode(JOYSTICK_INPUT_DOWN)}");
                Console.WriteLine($"Toggle up pin open: {controller.IsPinOpen(JOYSTICK_INPUT_UP)}");
                //Console.WriteLine($"Toggle down pin open: {controller.IsPinOpen(JOYSTICK_INPUT_DOWN)}");
                Console.WriteLine($"Toggle up pin value: {controller.Read(JOYSTICK_INPUT_UP)}");
                //Console.WriteLine($"Toggle down pin value: {controller.Read(JOYSTICK_INPUT_DOWN)}");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"GPIO not supported on this platform ({ex})");
            }
        }

        void HandleToggleChange(bool isOn)
        {
            try
            {
                RequestVRCToggle? rToggle = RequestVRCControl.GetControlByID("Switch") as RequestVRCToggle;

                if (rToggle == null)
                    return;

                if (rToggle.IsOn == isOn)
                    return;

                Console.WriteLine($"Running!");

                Console.WriteLine($"Toggle state: {isOn}");
                Console.WriteLine($"Toggle up pin mode: {controller.GetPinMode(JOYSTICK_INPUT_UP)}");
                //Console.WriteLine($"Toggle down pin mode: {controller.GetPinMode(JOYSTICK_INPUT_DOWN)}");
                Console.WriteLine($"Toggle up pin open: {controller.IsPinOpen(JOYSTICK_INPUT_UP)}");
                //Console.WriteLine($"Toggle down pin open: {controller.IsPinOpen(JOYSTICK_INPUT_DOWN)}");
                Console.WriteLine($"Toggle up pin value: {controller.Read(JOYSTICK_INPUT_UP)}");
                //Console.WriteLine($"Toggle down pin value: {controller.Read(JOYSTICK_INPUT_DOWN)}");

                rToggle.IsOn = isOn;
                rToggle.HandleRequest(null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Toggle Error ({ex.Message})");
            }
        }

        static DateTime knobTS;

        static void HandleKnobChange(bool isClockwise)
        {
            try
            {
                if (DateTime.UtcNow < knobTS.AddMilliseconds(50))
                    return;

                RequestVRCKnob? rKnob = RequestVRCControl.GetControlByID("Knob") as RequestVRCKnob;

                if (rKnob == null)
                    return;

                float increment = 1.0f / 16; //16 segments in led display

                if (!isClockwise)
                    increment = -increment;

                rKnob.Value += increment;
                if (rKnob.Value < 0)
                    rKnob.Value += 1;
                else if (rKnob.Value > 1)
                    rKnob.Value -= 1;

                rKnob.HandleRequest(null);
                knobTS = DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Knob Error ({ex.Message})");
            }
        }

        static void HandleButtonChange(bool isPressed)
        {
            try
            {
                RequestVRCButton? rButton = RequestVRCControl.GetControlByID("Button") as RequestVRCButton;

                if (rButton == null)
                    return;

                rButton.IsPressed = isPressed;
                rButton.HandleRequest(null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Button Error ({ex.Message})");
            }
        }

        public void Dispose()
        {
            controller?.Dispose();
        }

        public static void SetPin(int pinNumber, bool value)
        {
            if (Instance == null)
                return;

            if (Instance.controller == null)
                return;

            Instance.controller.Write(pinNumber, (value) ? PinValue.High : PinValue.Low);
        }
    }
}
