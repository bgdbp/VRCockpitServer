using System.Diagnostics;

namespace VRCockpitServer.CommClasses
{
    internal class RequestVRCButton : RequestVRCControl
    {
        public bool IsPressed { get; set; }

        public override Task HandleRequest()
        {
            base.HandleRequest();
            Console.WriteLine($"RequestButton: {ControlID} IsPressed: {IsPressed}");
            //if (IsPressed)
            //  GPIOManager.SetPin(pinMap[ButtonID])
            //else
            //  unset pin...

            return Task.CompletedTask;
        }
    }
}
