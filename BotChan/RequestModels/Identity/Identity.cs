using Newtonsoft.Json;

namespace BotChan.RequestModels.Identity
{
    class Identity
    {
        [JsonProperty(PropertyName = "token", Required = Required.Always)]
        public string AuthenticationToken { get; set; }

        [JsonProperty(PropertyName = "properties", Required = Required.Always)]
        public IdentityProperties Properties { get; set; }

        [JsonProperty(PropertyName = "compress", NullValueHandling = NullValueHandling.Ignore)]
        public bool Compress { get; set; }
    }
}
