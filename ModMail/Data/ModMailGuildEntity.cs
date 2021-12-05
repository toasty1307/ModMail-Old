using System.ComponentModel.DataAnnotations;

namespace ModMail.Data;

public class ModMailGuildEntity : global::Commands.Data.GuildEntity
{
    public ulong ModMailCategory { get; set; }
    public ulong ModMailLogChannel { get; set; }
    public List<ModMailThreadEntity> ModMailThreads { get; set; }

    private ModMailGuildEntity(global::Commands.Data.GuildEntity other)
    {
        Commands = other.Commands;
        Groups = other.Groups;
        Prefix = other.Prefix;
        GuildId = other.GuildId;
    }
    
    // ReSharper disable once UnusedMember.Local
    private ModMailGuildEntity() { }

    public static ModMailGuildEntity FromGuildEntity(global::Commands.Data.GuildEntity guildEntity) =>
        guildEntity is ModMailGuildEntity ge ? ge : new ModMailGuildEntity(guildEntity);
}