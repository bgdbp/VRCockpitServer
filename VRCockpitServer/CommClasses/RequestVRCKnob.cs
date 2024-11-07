using System.Diagnostics;

namespace VRCockpitServer.CommClasses
{
    internal class RequestVRCKnob : RequestVRCControl
    {
        public float Value { get; set; }

        public override Task HandleRequest()
        {
            base.HandleRequest();
            Console.WriteLine($"RequestKnob: {ControlID} Value: {Value}");
            //if (IsPressed)
            //  GPIOManager.SetPin(pinMap[ButtonID])
            //else
            //  unset pin...

            return Task.CompletedTask;
        }
    }
}
