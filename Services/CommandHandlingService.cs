using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SharpBot.Services
{
    public class CommandHandlingService
    {
        private readonly IConfiguration _config;
        private readonly CommandService _commands;
        private readonly IServiceProvider _services;
        private readonly DiscordSocketClient _client;

        public CommandHandlingService(IServiceProvider services)
        {
            _services = services;
            _commands = services.GetRequiredService<CommandService>();
            _client = services.GetRequiredService<DiscordSocketClient>();
            _config = services.GetRequiredService<IConfiguration>();

            _client.MessageReceived += MessageReceivedAsync;
            _commands.CommandExecuted += CommandExecutedAsync;
        }

        public async Task InitializeAsync()
        {
            // Automatically retrieve all public classes that inherit ModuleBase
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        public async Task MessageReceivedAsync(SocketMessage rawMessage)
        {
            // Ignore system messages or messages from bots (others/yourself)
            if (!(rawMessage is SocketUserMessage message) ||
                message.Source != MessageSource.User) return;

            // Check if the message has the needed prefix
            string commandPrefix =_config["commandPrefix"];
            int prefixIndex = 0;
            if (!message.HasStringPrefix(commandPrefix, ref prefixIndex)) return;

            SocketCommandContext context = new SocketCommandContext(_client, message);

            // Result handled with "CommandExecutedAsync"
            await _commands.ExecuteAsync(context, prefixIndex, _services);
        }

        public async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            // Command not found or is successful (Do nothing)
            if (!command.IsSpecified || result.IsSuccess) return;

            // Command Failed (Send a message)
            await context.Channel.SendMessageAsync($"Error: {result}");
        }
    }
}