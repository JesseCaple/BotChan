﻿using Newtonsoft.Json;

namespace BotChan.ResponseModels
{
    class Hello
    {
        [JsonProperty(PropertyName = "heartBeat_interval")]
        public int HeartBeatInterval { get; set; }
    }
}
