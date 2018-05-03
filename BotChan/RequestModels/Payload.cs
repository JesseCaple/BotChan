using System;
using Newtonsoft.Json;

namespace BotChan.RequestModels
{
    class Payload
    {
        [JsonProperty(PropertyName = "op", Required = Required.Always)]
        public Opcode Opcode { get; set; }

        [JsonProperty(PropertyName = "d", NullValueHandling = NullValueHandling.Ignore)]
        public Object EventData { get; set; }

        [JsonProperty(PropertyName = "s", NullValueHandling = NullValueHandling.Ignore)]
        public int? SequenceNumber { get; set; }

        [JsonProperty(PropertyName = "t", NullValueHandling = NullValueHandling.Ignore)]
        public string EventName { get; set; }
    }
}
