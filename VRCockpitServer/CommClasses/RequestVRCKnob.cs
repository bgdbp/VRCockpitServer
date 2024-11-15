using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace VRCockpitServer.CommClasses
{
    internal class RequestVRCKnob : RequestVRCControl
    {
        public float Value { get; set; }

        public override Task HandleRequest()
        {
            base.HandleRequest();
            Console.WriteLine($"RequestKnob: {ControlID} Value: {Value}");

            float correctedValue;
            if (Value <= 0.5f)
            {
                correctedValue = Single.Lerp(0, 0.2f, Value);
            }
            else
            {
                correctedValue = Single.Lerp(0.2f, 1, (Value - 0.5f) * 2);
            }

            GPIOManager.SetPwmPin(4, correctedValue);

            return Task.CompletedTask;
        }
    }
}
