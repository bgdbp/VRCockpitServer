using System.Diagnostics;

namespace VRCockpitServer.CommClasses
{
    internal class RequestVRCControl : VRCRequest
    {
        public required string ControlID { get; set; }

        public override Task HandleRequest()
        {
            Console.WriteLine($"ControlID: {ControlID}");

            return Task.CompletedTask;
        }
    }
}
