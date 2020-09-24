using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Victoria;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharpBot.Services;

namespace SharpBot {
    class Program {
        private ServiceProvider _services;

        static void Main (string[] args) =>
            new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync () {
            using (_services = ConfigureServices())
            {
                DiscordSocketClient client = _services.GetRequiredService<DiscordSocketClient>();
                IConfiguration configuration = _services.GetRequiredService<IConfiguration>();

                // TODO: Learn more about logging services!
                client.Log += Log;
                _services.GetRequiredService<CommandService>().Log += Log;

                await client.LoginAsync(TokenType.Bot, configuration["botToken"]);
                await client.StartAsync();

                client.Ready += InitializeServices;

                await Task.Delay(Timeout.Infinite);
            }
        }

        private ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton(BuildConfiguration())
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                .AddSingleton<HttpClient>()
                .AddSingleton<ImageService>()
                .AddLavaNode()
                .BuildServiceProvider();
        }

        private async Task InitializeServices() {
            await _services.GetRequiredService<CommandHandlingService>().InitializeAsync();
            LavaNode lavaNode = _services.GetRequiredService<LavaNode>();
            if (!lavaNode.IsConnected) await lavaNode.ConnectAsync();
            lavaNode.OnLog += Log;
        }

        private Task Log(LogMessage message) {
            Console.WriteLine(message.ToString());
            return Task.CompletedTask;
        }

        private IConfiguration BuildConfiguration()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(path: "config.json", optional: false, reloadOnChange: true)
                .Build();
        }
    }
}