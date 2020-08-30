using System;
using System.Threading.Tasks;
using Victoria;
using Victoria.Enums;
using Victoria.Responses.Rest;
using Discord;
using Discord.Audio;
using Discord.Commands;

namespace SharpBot.Modules
{
    [Summary("Commands for audio related tasks")]
    public class Audio : ModuleBase<SocketCommandContext>
    {
        public LavaNode LavaNode { get; set; }

        [Command("join")]
        [Summary("Have Sharp Bot join the channel")]
        public async Task Join(IVoiceChannel voiceChannel = null)
        {
            voiceChannel = voiceChannel ?? (Context.User as IVoiceState)?.VoiceChannel;
            if (voiceChannel == null) {
                await ReplyAsync(
                    "User must be in a voice channel, or a voice channel " +
                    "must be passed as an argument.");
                return;
            }

            await LavaNode.JoinAsync(voiceChannel, Context.Channel as ITextChannel);
        }

        [Command("leave")]
        [Summary("Have Sharp Bot leave the channel")]
        public async Task Leave(IVoiceChannel voiceChannel = null)
        {
            voiceChannel = voiceChannel ?? (Context.User as IVoiceState)?.VoiceChannel;
            if (voiceChannel == null) {
                await ReplyAsync(
                    "User must be in a voice channel, or a voice channel " +
                    "must be passed as an argument.");
                return;
            }

            await LavaNode.LeaveAsync(voiceChannel);
        }

        [Command("play")]
        [Summary("Have Sharp Bot sing :D")]
        public async Task Play([Remainder] string query) {
            LavaPlayer player = LavaNode.GetPlayer(Context.Guild);
            SearchResponse results = await LavaNode.SearchYouTubeAsync(query);
            if (results.LoadStatus == LoadStatus.NoMatches)
            {
                await ReplyAsync("No Matches");
                return;
            }
            var track = results.Tracks[0];
            await player.PlayAsync(track);
            await ReplyAsync("Playing");
        }
    }
}