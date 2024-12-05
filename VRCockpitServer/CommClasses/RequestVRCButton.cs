using System.Diagnostics;

namespace VRCockpitServer.CommClasses
{
    using static GPIOManager;

    internal class RequestVRCButton : RequestVRCControl
    {
        public bool IsPressed { get; set; }

        public override Task HandleRequest(UserSession? user)
        {

            base.HandleRequest(user);
            Console.WriteLine($"RequestButton: {ControlID} IsPressed: {IsPressed}");

            SetPin(BUTTON_OUTPUT, IsPressed);

            return Task.CompletedTask;
        }
    }
}
