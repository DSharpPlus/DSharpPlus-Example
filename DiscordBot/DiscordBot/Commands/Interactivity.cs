using DiscordBot.Entities;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DiscordBot.Commands
{
    [Group("interactivity"), Aliases("i")]
    internal class Interactivity
    {
        const string ConfirmRegex = "\\b[Yy][Ee]?[Ss]?\\b|\\b[Nn][Oo]?\\b";
        const string YesRegex = "[Yy][Ee]?[Ss]?";
        const string NoRegex = "[Nn][Oo]?";

        Dependencies dep;
        public Interactivity(Dependencies d)
        {
            this.dep = d;
        }

        [Command("confirmation")]
        public async Task ConfirmationAsync(CommandContext ctx)
        {
            await ctx.RespondAsync("Are you sure?");
            var m = await dep.Interactivity.WaitForMessageAsync(
                x => x.Channel.Id == ctx.Channel.Id
                && x.Author.Id == ctx.Member.Id
                && Regex.IsMatch(x.Content, ConfirmRegex));

            if (Regex.IsMatch(m.Message.Content, YesRegex))
                await ctx.RespondAsync("Confirmation Received");
            else
                await ctx.RespondAsync("Confirmation Denied");
        }
    }
}
