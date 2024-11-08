using System.Diagnostics;

namespace VRCockpitServer.CommClasses
{
    internal class RequestVRCControl : VRCRequest
    {
        public required string ControlID { get; set; }
        public static Dictionary<string, RequestVRCControl> ControlStates = [];

        public override Task HandleRequest()
        {
            ControlStates[ControlID] = this;

            //broadcast new state to other connections
            //ConnectionsManager.BroadcastResponse(this, ignoredSender: connection);

            return Task.CompletedTask;
        }
    }
}
