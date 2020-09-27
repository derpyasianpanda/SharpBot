using System;
using System.Threading.Tasks;
using Discord.Commands;
using SharpBot.Services;

namespace SharpBot.Modules
{
    [Summary("Extra Commands")]
    public class Misc : ModuleBase<SocketCommandContext>
    {
        public ImageService ImageService { get; set; }

        [Command("info")]
        [Summary("Info about this bot")]
        public async Task Info()
            => await ReplyAsync(
                $"Hello, I am a bot called " +
                Context.Client.CurrentUser.Username +
                " written in Discord.Net");

        [Command("deepfake", RunMode = RunMode.Async)]
        [Summary("Retrieves a randomly generated portrait from \"This Person Doesn't Exist\"")]
        public async Task Deepfake()
        {
            await ReplyAsync("Retrieving a completely generated face. The following image "+
            "is not real.");

            await Context.Channel.SendFileAsync(
                await ImageService.GetUnprotectedImage("thispersondoesnotexist.com/image"),
                "deepfake.jpg");
        }

        [Command("image", RunMode = RunMode.Async)]
        [Summary("Retrieves a random image from lorem picsum")]
        public async Task Image(params string[] effects)
        {
            string url = $"picsum.photos/500/?{String.Join('&', effects)}";

            await ReplyAsync("Retreiving your random image 😊");
            await Context.Channel.SendFileAsync(
                await ImageService.GetUnprotectedImage(url), "image.jpg");
            await Task.Delay(10);
        }

        [Command("image", RunMode = RunMode.Async)]
        [Summary("Retrieves a random image from lorem picsum")]
        public async Task ImageSimple(int size, params string[] effects)
        {
            string url = $"picsum.photos/{size}/{size}/?{String.Join('&', effects)}";

            await ReplyAsync("Retreiving your random image 😊");
            await Context.Channel.SendFileAsync(
                await ImageService.GetUnprotectedImage(url), "image.jpg");
            await Task.Delay(10);
        }

        [Command("image", RunMode = RunMode.Async)]
        [Summary("Retrieves a random image from lorem picsum")]
        public async Task ImageAdvanced(int width, int height, params string[] effects)
        {
            string url = $"picsum.photos/{width}/{height}/?{String.Join('&', effects)}";

            await ReplyAsync("Retreiving your random image 😊");
            await Context.Channel.SendFileAsync(
                await ImageService.GetUnprotectedImage(url), "image.jpg");
            await Task.Delay(10);
        }

        // Why did I hardcode this lol
        // Also I'm sorry if this truly offends anyone.
        [Command("gay")]
        [Summary("Don't do it")]
        public async Task Gay()
            => await ReplyAsync(
                "Just to let you know, almost everyone (except for you " +
                "and your idiotic friends it would seem) finds “gay”, " +
                "“autistic”, and “retarded” offensive. Using something that " +
                "people cannot control about themselves as an insult is honestly " +
                "disgusting. Also people who are autistic aren't bad people, or " +
                "“retarded”, or any of those things. They are people and they should " +
                "be treated like people. Just like people who are gay. Using gay as an " +
                "insult is not funny or anything close to it. That is basically saying " +
                "that people who are gay are automatically bad people. So if you did think " +
                "that people who aren't straight are automatically in some way “bad” or " +
                "“not normal”, then just think about how you could be friends with one. " +
                "Yeah. No one chooses who they love. Using any of these things as an insult " +
                "is disgusting and if you keep choosing to use them as insults, then you " +
                "will make many people offended and very angry at you. Using this word " +
                "choice will make you lose friends. So screw you and your privileged " +
                "opinions and life. Hope to never see you again GAYBOWSA!"
            );
    }
}