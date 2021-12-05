using System.ComponentModel.DataAnnotations;

namespace ModMail.Data;

public class ModMailThreadEntity
{
    [Key]
    public ulong Recepient { get; init; }
    public ulong Channel { get; init; }
    public ModMailGuildEntity GuildEntity { get; init; }
    public ulong GuildId { get; init; }
}