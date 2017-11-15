using DiscordBot.Entities;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Commands
{
    [Group("owner"), Aliases("o"), RequireOwner]
    internal class Owner
    {
        Dependencies dep;
        public Owner(Dependencies d)
        {
            this.dep = d;
        }

        [Command("shutdown")]
        public async Task ShutdownAsync(CommandContext ctx)
        {
            await ctx.RespondAsync("Shutting down!");
            dep.Cts.Cancel();
        }
    }
}
