using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BotChan.RequestModels.Identity
{
    class IdentityProperties
    {
        [JsonProperty(PropertyName = "$os", Required = Required.Always)]
        public string OperatingSystem {get;set;}

        [JsonProperty(PropertyName = "$browser", Required = Required.Always)]
        public string Browser { get; set; }

        [JsonProperty(PropertyName = "$device", Required = Required.Always)]
        public string Device { get; set; }
    }
}
