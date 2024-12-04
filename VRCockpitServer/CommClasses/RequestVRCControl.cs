using System.Diagnostics;

namespace VRCockpitServer.CommClasses
{
    internal class RequestVRCControl : VRCRequest
    {
        public required string ControlID { get; set; }
        public static Dictionary<string, RequestVRCControl> ControlStates = []; //<ControlID, RequestVRCControl
        public bool IsInitialSync { get; set; }


        public static RequestVRCControl GetControlByID(string controlID)
        {
            return ControlStates[controlID];
        }

        public override Task HandleRequest(UserSession? user)
        {
            // if the server already has an initial state,
            // send that to the client.
            if (IsInitialSync && ControlStates.TryGetValue(ControlID, out RequestVRCControl? currentState))
            {
                user.SendRequest(currentState);
                return Task.CompletedTask;
            }

            ControlStates[ControlID] = this;

            //if more than one client is connected, let them know
            //about this state change.
            UserSession.BroadcastRequest(this, ignoredUser: user);

            return Task.CompletedTask;
        }
    }
}
