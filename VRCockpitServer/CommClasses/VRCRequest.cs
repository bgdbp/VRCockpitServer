using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace VRCockpitServer.CommClasses
{
    [JsonDerivedType(typeof(RequestVRCControl), typeDiscriminator: 0)]
    [JsonDerivedType(typeof(RequestVRCKnob), typeDiscriminator: 1)]
    [JsonDerivedType(typeof(RequestVRCToggle), typeDiscriminator: 2)]
    [JsonDerivedType(typeof(RequestVRCButton), typeDiscriminator: 3)]
    internal abstract class VRCRequest
    {
        public abstract Task HandleRequest();
    }
}
