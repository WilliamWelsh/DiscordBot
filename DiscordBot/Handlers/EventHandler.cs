﻿using System;
using Discord;
using System.Linq;
using Discord.Commands;
using System.Reflection;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace Gideon
{
    class CommandHandler
    {
        DiscordSocketClient _client;
        CommandService _service;

        public async Task InitializeAsync(DiscordSocketClient client)
        {
            _client = client;
            _service = new CommandService();
            await _service.AddModulesAsync(Assembly.GetEntryAssembly());
            _client.MessageReceived += HandleCommandAsync;

            _client.UserBanned += HandleUserBanned;
            _client.UserUnbanned += HandleUserUnbanned;

            _client.UserJoined += HandleUserJoining;
            _client.UserLeft += HandleUserLeaving;
        }

        private async Task HandleUserUnbanned(SocketUser arg1, SocketGuild arg2) => await arg2.GetTextChannel(294699220743618561).SendMessageAsync("", false, Config.Utilities.Embed("Pardon", $"{arg1} has been unbanned.", new Color(31, 139, 76), "", arg1.GetAvatarUrl()));

        private async Task HandleUserBanned(SocketUser arg1, SocketGuild arg2)
        {
            var bans = arg2.GetBansAsync().Result.ToList();
            string reason = "";
            foreach (var ban in bans)
                if (ban.User.Id == arg1.Id)
                    reason = ban.Reason;
            if (reason == "")
                await arg2.GetTextChannel(294699220743618561).SendMessageAsync("", false, Config.Utilities.Embed("Ban", $"{arg1} has been banned.", new Color(231, 76, 60), "", arg1.GetAvatarUrl()));
            else
                await arg2.GetTextChannel(294699220743618561).SendMessageAsync("", false, Config.Utilities.Embed("Ban", $"{arg1} has been banned for {reason}.", new Color(231, 76, 60), "", arg1.GetAvatarUrl()));
        }

        private async Task HandleUserJoining(SocketGuildUser arg)
        {
            string desc = $"{arg} has joined the server.";
            if (UserAccounts.GetAccount(arg).level != 0)
            {
                string rank = Config.RankHandler.LevelToRank(UserAccounts.GetAccount(arg).level);
                await (arg as IGuildUser).AddRoleAsync(arg.Guild.Roles.FirstOrDefault(x => x.Name == rank));
                desc += $" Their rank has been restored to {rank}.";
            }
            await arg.Guild.GetTextChannel(294699220743618561).SendMessageAsync("", false, Config.Utilities.Embed("New User", desc, new Color(31, 139, 76), "", arg.GetAvatarUrl()));
        }

        private async Task HandleUserLeaving(SocketGuildUser arg) => await arg.Guild.GetTextChannel(294699220743618561).SendMessageAsync("", false, Config.Utilities.Embed("User Left", $"{arg} has left the server.", new Color(231, 76, 60), "", arg.GetAvatarUrl()));

        private async Task HandleCommandAsync(SocketMessage s)
        {
            SocketUserMessage msg = s as SocketUserMessage;
            if (msg == null || msg.Author.IsBot) return;

            var context = new SocketCommandContext(_client, msg);

            await Config.RankHandler.TryToGiveUserXP(context, msg.Author);

            int argPos = 0;
            if (msg.HasStringPrefix("!", ref argPos))
                await _service.ExecuteAsync(context, argPos);

            string m = msg.Content.ToLower();

            // Answer trivia
            if (m == "a" || m == "b" || m == "c" || m == "d")
                if(context.Channel.Id == 518846214603669537)
                    await Config.MinigameHandler.Trivia.AnswerTrivia((SocketGuildUser)msg.Author, context, m);

            // Print a lennyface
            if (m.Contains("lennyface"))
                await context.Channel.SendMessageAsync("( ͡° ͜ʖ ͡°)");

            string[] spellingMistakes = { "should of", "would of", "wouldnt of", "wouldn't of", "would not of", "couldnt of", "couldn't of", "could not of", "better of", "shouldnt of", "shouldn't of", "should not of", "alot", "could of" };
            string[] spellingFix = { "should have", "would have", "wouldn't have", "wouldn't have", "would not have", "couldn't have", "couldn't have", "could not have", "better have", "shouldn't have", "shouldn't have", "should not have", "a lot", "could have" };

            for (int i = 0; i < spellingMistakes.Length; i++)
                if (m.Contains(spellingMistakes[i]))
                    await msg.Channel.SendMessageAsync(spellingFix[i] + "*");

            if (s.Channel.Name.StartsWith("@"))
                Console.WriteLine($" ----------\n DIRECT MESSAGE\n From: {s.Channel}\n {s}\n ----------");
        }
    }
}