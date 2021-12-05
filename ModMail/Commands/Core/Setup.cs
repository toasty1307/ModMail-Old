using Commands.CommandsStuff;
using DSharpPlus;
using DSharpPlus.Entities;
using ModMail.Data;

namespace ModMail.Commands.Core;

public class Setup : Command
{
    public override string GroupName => "Core";

    public Setup(DiscordClient client) : base(client) { }

    
    public override async Task Run(InteractionContext ctx)
    { 
        var overrides = ctx.Guild.Roles.Values.Where(x => x.Permissions.HasPermission(Permissions.ManageChannels))
            .Select(x => new DiscordOverwriteBuilder(x).Allow(Permissions.ManageMessages)).ToList();
        overrides.Add(new DiscordOverwriteBuilder(ctx.Guild.EveryoneRole).Deny(Permissions.All));
        var category = await ctx.Guild.CreateChannelAsync("ModMail", ChannelType.Category, overwrites: overrides);
        var logChannel = await ctx.Guild.CreateChannelAsync("Logs", ChannelType.Text, category);
        var guildEntity = (ModMailGuildEntity)Extension.Provider.Get(ctx.Guild);
        guildEntity.ModMailCategory = category.Id;
        guildEntity.ModMailLogChannel = logChannel.Id;
        guildEntity.ModMailThreads = new List<ModMailThreadEntity>();
        Extension.Provider.Set(guildEntity);
        var embed = new DiscordEmbedBuilder()
            .WithTitle("ModMail Setup Complete")
            .WithDescription(
                "This is now the logs channel for ModMail\nHowever you can change this using the `settings logchannel` command")
            .WithColor(new DiscordColor("2F3136"));
        
        await category.ModifyPositionAsync(0);
        await logChannel.SendMessageAsync(embed);
        await ctx.ReplyAsync("Setup Complete!");
    }

    public override async Task Run(MessageContext ctx)
    {
        var overrides = ctx.Guild.Roles.Values.Where(x => x.Permissions.HasPermission(Permissions.ManageChannels))
            .Select(x => new DiscordOverwriteBuilder(x).Allow(Permissions.ManageMessages)).ToList();
        overrides.Add(new DiscordOverwriteBuilder(ctx.Guild.EveryoneRole).Deny(Permissions.All));
        var category = await ctx.Guild.CreateChannelAsync("ModMail", ChannelType.Category, overwrites: overrides);
        var logChannel = await ctx.Guild.CreateChannelAsync("Logs", ChannelType.Text, category);
        var guildEntity = (ModMailGuildEntity)Extension.Provider.Get(ctx.Guild);
        guildEntity.ModMailCategory = category.Id;
        guildEntity.ModMailLogChannel = logChannel.Id;
        guildEntity.ModMailThreads = new List<ModMailThreadEntity>();
        Extension.Provider.Set(guildEntity);
        var embed = new DiscordEmbedBuilder()
            .WithTitle("ModMail Setup Complete")
            .WithDescription(
                "This is now the logs channel for ModMail\nHowever you can change this using the `settings logchannel` command")
            .WithColor(new DiscordColor("2F3136"));
        
        await category.ModifyAsync(x => x.Position = 0);
        await logChannel.SendMessageAsync(embed);
        await ctx.ReplyAsync("Setup Complete!");
    }
}