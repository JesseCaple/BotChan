using Newtonsoft.Json;

namespace BotChan.ResponseModels
{
    class GatewayStart
    {
        [JsonProperty(PropertyName = "url")]
        public string GatewayURL { get; set; }
    }
}
