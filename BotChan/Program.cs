using BotChan.RequestModels;
using BotChan.ResponseModels;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace BotChan
{
    class Program
    {
        static void Main(string[] args)
        {
            RunAsync().Wait();
        }

        static async Task RunAsync()
        {
            var config = await Config.OpenAsync("app.cfg");
            using (var gateway = await Gateway.OpenAsync(config.BotToken))
            {

            }
        }
    }
}
