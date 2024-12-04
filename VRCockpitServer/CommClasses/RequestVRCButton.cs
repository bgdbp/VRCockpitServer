using System.Diagnostics;

namespace VRCockpitServer.CommClasses
{
    internal class RequestVRCButton : RequestVRCControl
    {
        public bool IsPressed { get; set; }

        public override Task HandleRequest(UserSession? user)
        {
            base.HandleRequest(user);
            Console.WriteLine($"RequestButton: {ControlID} IsPressed: {IsPressed}");
            GPIOManager.SetButton(IsPressed);

            return Task.CompletedTask;
        }
    }
}
