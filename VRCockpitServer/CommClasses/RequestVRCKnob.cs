using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace VRCockpitServer.CommClasses
{
    internal class RequestVRCKnob : RequestVRCControl
    {
        public float Value { get; set; }

        public override Task HandleRequest(UserSession? user)
        {
            base.HandleRequest(user);

            Console.WriteLine($"RequestKnob: {ControlID} Value: {Value}");
            LEDRingManager.Instance?.SetKnobValue((byte)(Value * 16));

            return Task.CompletedTask;
        }
    }
}
