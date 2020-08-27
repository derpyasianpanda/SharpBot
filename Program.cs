using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharpBot.Services;

namespace SharpBot {
    class Program {
        static void Main (string[] args) =>
            new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync () {
            using (ServiceProvider services = ConfigureServices())
            {
                DiscordSocketClient client = services.GetRequiredService<DiscordSocketClient>();
                IConfiguration configuration = services.GetRequiredService<IConfiguration>();

                // TODO: Learn more about logging services!
                client.Log += Log;
                services.GetRequiredService<CommandService>().Log += Log;

                await client.LoginAsync(TokenType.Bot, configuration["botToken"]);
                await client.StartAsync();

                await services.GetRequiredService<CommandHandlingService>().InitializeAsync();

                await Task.Delay(Timeout.Infinite);
            }
        }

        private ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton(CreateConfiguration())
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                .AddSingleton<HttpClient>()
                .AddSingleton<ImageService>()
                .BuildServiceProvider();
        }

        private Task Log(LogMessage message) {
            Console.WriteLine(message.ToString());
            return Task.CompletedTask;
        }

        private IConfiguration CreateConfiguration()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(path: "config.json", optional: false, reloadOnChange: true)
                .Build();
        }
    }
}