namespace ModMail.Data;

public class GuildEntity : Commands.Data.GuildEntity
{
    private GuildEntity(Commands.Data.GuildEntity other)
    {
        Commands = other.Commands;
        Groups = other.Groups;
        Prefix = other.Prefix;
        GuildId = other.GuildId;
    }
    
    // ReSharper disable once UnusedMember.Local
    private GuildEntity() { }

    public static GuildEntity FromGuildEntity(Commands.Data.GuildEntity guildEntity) =>
        guildEntity is GuildEntity ge ? ge : new GuildEntity(guildEntity);
}