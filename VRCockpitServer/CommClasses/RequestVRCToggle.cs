using System.Diagnostics;

namespace VRCockpitServer.CommClasses
{
    using static GPIOManager;

    internal class RequestVRCToggle : RequestVRCControl
    {
        public bool IsOn { get; set; }

        public override Task HandleRequest(UserSession? user)
        {
            base.HandleRequest(user);

            Console.WriteLine($"RequestToggle: {ControlID} IsOn: {IsOn}");

            SetPin(JOYSTICK_OUTPUT_UP, IsOn);
            SetPin(JOYSTICK_OUTPUT_DOWN, !IsOn);

            return Task.CompletedTask;
        }
    }
}
