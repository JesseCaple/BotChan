using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BotChan.RequestModels.Identity
{
    class IdentityResume
    {
        [JsonProperty(PropertyName = "token", Required = Required.Always)]
        public string SessionToken { get; set; }

        [JsonProperty(PropertyName = "session_id", Required = Required.Always)]
        public string SessionId { get; set; }

        [JsonProperty(PropertyName = "seq", Required = Required.Always)]
        public string SequenceNumber { get; set; }
    }
}
