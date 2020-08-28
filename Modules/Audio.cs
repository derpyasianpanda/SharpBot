using System.Net.Sockets;
using System.Threading.Tasks;
using Discord;
using Discord.Audio;
using Discord.Commands;

namespace SharpBot.Modules
{
    public class Audio : ModuleBase<SocketCommandContext>
    {
        [Command("join", RunMode = RunMode.Async)]
        public async Task JoinChannel(IVoiceChannel channel = null)
        {
            channel = channel ?? (Context.User as IGuildUser)?.VoiceChannel;
            if (channel == null) {
                await ReplyAsync(
                    "User must be in a voice channel, or a voice channel " +
                    "must be passed as an argument.");
                return;
            }

            IAudioClient audioClient = await channel.ConnectAsync();
        }

        [Command("play")]
        public Task PlayAsync(string url) {
            return Task.CompletedTask;
        }
    }
}