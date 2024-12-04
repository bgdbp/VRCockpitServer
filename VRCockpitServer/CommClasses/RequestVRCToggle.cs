using System.Diagnostics;

namespace VRCockpitServer.CommClasses
{
    internal class RequestVRCToggle : RequestVRCControl
    {
        public bool IsOn { get; set; }

        public override Task HandleRequest(UserSession? user)
        {
            base.HandleRequest(user);

            Console.WriteLine($"RequestToggle: {ControlID} IsOn: {IsOn}");
            GPIOManager.SetToggle(IsOn);

            return Task.CompletedTask;
        }
    }
}
