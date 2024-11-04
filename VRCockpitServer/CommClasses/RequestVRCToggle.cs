using System.Diagnostics;

namespace VRCockpitServer.CommClasses
{
    internal class RequestVRCToggle : RequestVRCControl
    {
        public bool IsOn { get; set; }

        public override Task HandleRequest()
        {
            Console.WriteLine($"RequestToggle: {ControlID} IsOn: {IsOn}");
            //if (IsPressed)
            //  GPIOManager.SetPin(pinMap[ButtonID])
            //else
            //  unset pin...

            return Task.CompletedTask;
        }
    }
}
