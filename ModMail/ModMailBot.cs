using Commands;
using Commands.Utils;
using DSharpPlus;
using DSharpPlus.Entities;
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
        foreach (var (id, client) in _client.ShardClients)
        {
            var ext = client.UseCommands(new CommandsExtension(commandsConfig));
            ext.Registry.RegisterDefaults();
            ext.Registry.RegisterCommands(GetType().Assembly);
        }
        
        // update client's status
        await _client.UpdateStatusAsync(new DiscordActivity("ModMail", ActivityType.Watching));
        
        _client.Logger.LogDebug("Bot Up");
    }
}