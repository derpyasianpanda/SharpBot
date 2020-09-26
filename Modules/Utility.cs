using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Discord;
using Discord.Commands;
using SharpBot.Services;

namespace SharpBot.Modules
{
    [Summary("Utility commands")]
    public class Utility : ModuleBase<SocketCommandContext>
    {
        public ImageService ImageService { get; set; }
        public CommandService CommandService { get; set; }

        [Priority(1)]
        [Command("help module")]
        [Summary("Lists modules")]
        public async Task HelpModule()
        {
            EmbedBuilder embedBuilder = new EmbedBuilder();

            foreach (ModuleInfo module in CommandService.Modules)
            {
                embedBuilder.AddField(module.Name, module.Summary);
            }

            await ReplyAsync("Use \"$help module <module name>\" to get more help \n" +
                "Here's a list of modules: ", false, embedBuilder.Build());
        }

        [Priority(1)]
        [Command("help module")]
        [Summary("Lists commands in the given module")]
        public async Task HelpModule(string moduleName)
        {
            IEnumerable<ModuleInfo> modules = CommandService.Modules;
            EmbedBuilder embedBuilder = new EmbedBuilder();

            foreach (ModuleInfo module in modules)
            {
                if (module.Name.ToLower() == moduleName.ToLower()) {
                    foreach (CommandInfo command in module.Commands) {
                        string commandSummary = command.Summary ?? "No description available";
                        embedBuilder.AddField(command.Name, commandSummary);
                    }
                }
            }

            await ReplyAsync($"Here's a list of {moduleName} commands and their description: ",
                false, embedBuilder.Build());
        }

        [Command("help")]
        [Summary("Retrieves information about commands containing the query")]
        public async Task Help(string commandName = "")
        {
            if (commandName == "") {
                await HelpModule();
                return;
            }
            IEnumerable<CommandInfo> commands = CommandService.Commands
                .Where(command => command.Name.Contains(commandName));
            EmbedBuilder embedBuilder = new EmbedBuilder();

            foreach (CommandInfo command in commands) {
                string commandSummary = command.Summary ?? "No description available 😢";
                string commandParameters = String.Join(", ",
                    command.Parameters.Select(parameter =>
                        $"{parameter.Name} ({parameter.Type})"));

                commandParameters = commandParameters.Length > 0 ?
                    $"Parameters: {commandParameters}" : "";

                embedBuilder.AddField(command.Name,
                    $"{commandSummary}\n{commandParameters}");
            }

            await ReplyAsync(
                $"Here's a list of commands with {commandName} and their description: ",
                false, embedBuilder.Build());
        }

        [Command("clean", RunMode=RunMode.Async)]
        [Summary("Remove a given number of messages (default: 10). CAREFUL NOT TO BREAK LIMITS!")]
        [RequireUserPermission(ChannelPermission.ManageMessages)]
        public async Task Clean(int limit = 10) {
            if (limit < 1) {
                await ReplyAsync("You can't remove less than 1 message!");
                return;
            }

            int messagesLeft = await PurgeBase(limit);
            IEnumerable<IMessage> messages = await Context.Channel
                .GetMessagesAsync(Context.Message, Direction.Before, messagesLeft)
                .FlattenAsync();

            foreach (IMessage message in messages) {
                await Context.Channel.DeleteMessageAsync(message);
                // This delay is to prevent rate limiting
                await Task.Delay(1000);
            }
            await ReplyAsync($"Removed {limit} message(s)");
        }

        [Command("purge", RunMode=RunMode.Async)]
        [Summary("Removes \"all\" messages sent within the past 14 days.")]
        [RequireUserPermission(ChannelPermission.ManageMessages)]
        public async Task Purge() {
            await ReplyAsync("Removing messages sent within the last 14 days");
            int messagesLeft = 0;
            while (messagesLeft == 0) messagesLeft = await PurgeBase(1000);
        }

        private async Task<int> PurgeBase(int limit) {
            limit = limit > 10000 ? 10000 : limit;
            ITextChannel channel = (ITextChannel) Context.Channel;
            IEnumerable<IMessage> messages = (await channel
                .GetMessagesAsync(Context.Message, Direction.Before, limit)
                .FlattenAsync())
                .Where(message =>
                    (DateTimeOffset.UtcNow - message.Timestamp).TotalDays <= 14);
            await channel.DeleteMessagesAsync(messages);
            return limit - messages.Count();
        }
    }
}