using BotChan.RequestModels;
using BotChan.RequestModels.Identity;
using BotChan.ResponseModels;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BotChan
{
    class Gateway : IDisposable
    {
        private const string APIVersion = "6";
        private const int MaxPayloadSize = 4096;

        private readonly string botToken;
        private readonly CancellationTokenSource cancellation;
        private readonly SemaphoreSlim sempahore;
        private readonly ClientWebSocket socket;

        private Task heartbeatTask;

        private Gateway(string botToken)
        {
            this.botToken = botToken;
            cancellation = new CancellationTokenSource();
            sempahore = new SemaphoreSlim(1, 1);
            socket = new ClientWebSocket();
        }

        public static async Task<Gateway> OpenAsync(string botToken)
        {
            var gateway = new Gateway(botToken);
            await gateway.ConnectAsync();
            return gateway;
        }

        private async Task ConnectAsync()
        {
            Uri gatewayUri;
            using (var http = new HttpClient())
            {
                http.DefaultRequestHeaders.Add("Authorization", $"Bot {botToken}");
                var response = await http.GetAsync($"https://discordapp.com/api/v{APIVersion}/gateway/bot", cancellation.Token);
                var str = await response.Content.ReadAsStringAsync();
                var json = JsonConvert.DeserializeObject<GatewayStart>(str);
                gatewayUri = new Uri($"{json.GatewayURL}?v={APIVersion}&encoding=json");
            }

            var buffer = new ArraySegment<byte>(new byte[MaxPayloadSize]);
            await socket.ConnectAsync(gatewayUri, cancellation.Token);
            var payload = await ReceiveAsync();
            if (payload.Opcode != Opcode.Hello)
            {
                // TODO: handle error
            }
            var hello = JsonConvert.DeserializeObject<Hello>(payload.EventData.ToString());

            await SendAsync(new Payload
            {
                Opcode = Opcode.Identify,
                EventData = new Identity
                {
                    AuthenticationToken = botToken,
                    Properties = new IdentityProperties
                    {
                        OperatingSystem = "windows",
                        Browser = "BotChan",
                        Device = "BotChan"
                    },
                    Compress = false
                }
            });
            heartbeatTask = LoopHeartbeatAsync(hello.HeartBeatInterval);
        }

        private async Task LoopHeartbeatAsync(int interval)
        {
            do
            {
                await SendAsync(new Payload { Opcode = Opcode.Heartbeat });
                await Task.Delay(interval, cancellation.Token);
            }
            while (!cancellation.IsCancellationRequested);
        }

        private async Task<Payload> ReceiveAsync()
        {
            await sempahore.WaitAsync(cancellation.Token);
            try
            {
                using (var memory = new MemoryStream())
                {
                    var buffer = new byte[MaxPayloadSize];
                    WebSocketReceiveResult result;
                    do
                    {
                        result = await socket.ReceiveAsync(buffer, cancellation.Token);
                        await memory.WriteAsync(buffer, 0, result.Count);
                    }
                    while (!result.EndOfMessage);
                    var data = memory.ToArray();
                    string str = Encoding.UTF8.GetString(data, 0, data.Length);
                    return JsonConvert.DeserializeObject<Payload>(str);
                }
            }
            finally
            {
                sempahore.Release();
            }
        }

        private async Task SendAsync(Payload payload)
        {
            await sempahore.WaitAsync(cancellation.Token);
            try
            {
                var str = JsonConvert.SerializeObject(payload);
                var data = Encoding.UTF8.GetBytes(str);
                var length = data.Length;
                var offset = 0;
                do
                {
                    var count = length - offset;
                    if (count > MaxPayloadSize) count = MaxPayloadSize;
                    var segment = new ArraySegment<byte>(data, offset, count);
                    await socket.SendAsync(segment, WebSocketMessageType.Binary, offset < data.Length, cancellation.Token);
                    offset += count;
                }
                while (offset < data.Length);
            }
            finally
            {
                sempahore.Release();
            }
        }

        private async Task DisconnectAsync()
        {
            var timeout = new CancellationTokenSource(10000);
            await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Goodbye!", timeout.Token);
        }

        public void Dispose()
        {
            try
            {
                DisconnectAsync().Wait();
            }
            finally
            {
                cancellation.Cancel();
            }
        }


    }
}
