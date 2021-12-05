using Commands;
using Commands.CommandsStuff;
using Commands.Utils;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Logging;
using ModMail.Data;
using Newtonsoft.Json;
using LoggerFactory = Commands.Utils.LoggerFactory;

namespace ModMail;

public class ModMailBot
{
    private readonly DiscordShardedClient _client;
    private readonly Config _config;
    public ModMailBot()
    {
        _config = GetConfig("Config.json");
        _client = new DiscordShardedClient(new DiscordConfiguration
        {
            Token = _config.Token,
            MinimumLogLevel = LogLevel.Debug,
            LoggerFactory = new LoggerFactory(),
            Intents = DiscordIntents.All
        });
        _client.ChannelDeleted += OnChannelDeleted;
    }

    private async Task OnReady(DiscordClient sender)
    {
        var provider = sender.GetCommandsExtension().Provider;
        foreach (var (_, guild) in sender.Guilds)
        {
            var ge = (ModMailGuildEntity)provider.Get(guild);
            DiscordChannel catChannel;
            DiscordChannel logsChannel;
            var flag = false;
            if (guild.GetChannel(ge.ModMailCategory) is null)
            {
                var overrides = guild.Roles.Values.Where(x => x.Permissions.HasPermission(Permissions.ManageChannels))
                    .Select(x => new DiscordOverwriteBuilder(x).Allow(Permissions.ManageMessages)).ToList();
                overrides.Add(new DiscordOverwriteBuilder(guild.EveryoneRole).Deny(Permissions.All));
                catChannel = await guild.CreateChannelAsync("ModMail", ChannelType.Category, overwrites: overrides);
                ge.ModMailCategory = catChannel.Id;
                await catChannel.ModifyAsync(x => x.Position = 0);
                flag = true;
            }
            else
                catChannel = guild.GetChannel(ge.ModMailCategory);

            if (guild.GetChannel(ge.ModMailLogChannel) is null)
            {
                logsChannel = await guild.CreateChannelAsync("Logs", ChannelType.Text, catChannel);
                await logsChannel.SendMessageAsync("why you delete channel when i was slep");
                ge.ModMailLogChannel = logsChannel.Id;
            }
            else
                logsChannel = guild.GetChannel(ge.ModMailLogChannel);
            
            if (flag)
                await logsChannel.SendMessageAsync("why delete category when i slep");

            foreach (var modMailThreadEntity in ge.ModMailThreads ?? new List<ModMailThreadEntity>())
            {
                var exists = guild.GetChannel(modMailThreadEntity.Channel) is null;
                if (exists)
                    await guild.GetChannel(modMailThreadEntity.Channel).ModifyAsync(x => x.Parent = catChannel);
                else
                {
                    await logsChannel.SendMessageAsync($"why you delete {modMailThreadEntity.Recepient}'s thread?");
                    // todo tell user their channel was yeeted when bot slep
                }
            }
            
            provider.Set(ge);
        }
    }

    private async Task OnChannelDeleted(DiscordClient sender, ChannelDeleteEventArgs e)
    {
        var provider = sender.GetCommandsExtension().Provider;
        var guildEntity = (ModMailGuildEntity)provider.Get(e.Guild);
        if (e.Channel.Id == guildEntity.ModMailCategory)
        {
            var overrides = e.Guild.Roles.Values.Where(x => x.Permissions.HasPermission(Permissions.ManageChannels))
                .Select(x => new DiscordOverwriteBuilder(x).Allow(Permissions.ManageMessages)).ToList();
            overrides.Add(new DiscordOverwriteBuilder(e.Guild.EveryoneRole).Deny(Permissions.All));
            var catChannel = await e.Guild.CreateChannelAsync("ModMail", ChannelType.Category, overwrites: overrides);
            await catChannel.ModifyAsync(x => x.Position = 0);
            await e.Guild.Channels[guildEntity.ModMailLogChannel].SendMessageAsync("can ya stop trying to break me");
            await e.Guild.Channels[guildEntity.ModMailLogChannel].ModifyAsync(x => x.Parent = catChannel);

            async void Action(ModMailThreadEntity x) => await e.Guild.Channels[x.Channel].ModifyAsync(chn => chn.Parent = catChannel);

            guildEntity.ModMailThreads.ForEach(Action);
            guildEntity.ModMailCategory = catChannel.Id;
            provider.Set(guildEntity);
        }
        else if (e.Channel.Id == guildEntity.ModMailLogChannel)
        {
            var logs = await e.Guild.CreateChannelAsync("Logs", ChannelType.Text, e.Guild.Channels[guildEntity.ModMailCategory]);
            await logs.SendMessageAsync("well there goes all your logs");
            guildEntity.ModMailLogChannel = logs.Id;
            provider.Set(guildEntity);
        }
        // todo tell user their channel was yeeted
    }

    private Config GetConfig(string fileName)
    {
        var directory = Directory.GetCurrentDirectory();
        var fullPath = Path.Combine(directory, fileName);
        var reader = new StreamReader(fullPath);
        var jsonString = reader.ReadToEnd();
        var config = JsonConvert.DeserializeObject<Config>(jsonString);
        return config;
    }

    public async Task RunAsync()
    {
        await _client.StartAsync();
        var commandsConfig = new CommandsConfig
        {
            Invite = "no invite for you",
            Prefix = _config.Prefix,
            NonCommandEditable = true,
            Owners = _config.Owners,
            Provider = new GuildContext()
        };
        foreach (var (_, client) in _client.ShardClients)
        {
            var ext = client.UseCommands(new CommandsExtension(commandsConfig));
            ext.Registry.RegisterDefaults();
            ext.Registry.RegisterGroups(new Group[]
            {
                new()
                {
                    Name = "Core",
                    Description = "CoreCommands",
                    Guarded = true
                }
            });
            ext.Registry.RegisterCommands(GetType().Assembly);
            await OnReady(client);
        }
        
        await _client.UpdateStatusAsync(new DiscordActivity("ModMail", ActivityType.Watching));
        
        _client.Logger.LogDebug("Bot Up");
    }
}