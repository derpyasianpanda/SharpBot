using System;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

namespace DiscordBot {
    class Program {
        private DiscordSocketClient client;
        private IConfiguration config;

        static void Main (string[] args) =>
            new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync () {
            config = CreateConfig();

            client = new DiscordSocketClient();
            client.Log += Log;
            client.MessageReceived += MessageReceived;

            await client.LoginAsync(TokenType.Bot, config["botToken"]);
            await client.StartAsync();

            await Task.Delay(-1);
        }

        private Task Log(LogMessage message) {
            Console.WriteLine(message.ToString());
            return Task.CompletedTask;
        }

        private async Task MessageReceived(SocketMessage message) {
            if (message.Content == "!ping")
                await message.Channel.SendMessageAsync("Pong!");
        }

        private IConfiguration CreateConfig()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json")
                .Build();
        }
    }
}