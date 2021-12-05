using System.ComponentModel.DataAnnotations.Schema;
using Commands.Data;
using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;

namespace ModMail.Data;

public class GuildContext : DbContext, ISettingProvider
{
    [NotMapped] public List<ModMailGuildEntity> Cache { get; } = new();

    public DbSet<ModMailGuildEntity> Guilds { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost; Database=ModMail; Username=toasty; Password=toasty;");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ModMailGuildEntity>()
            .Property(x => x.Commands)
            .HasConversion(x => JsonConvert.SerializeObject(x),
                x => JsonConvert.DeserializeObject<Dictionary<DisabledCommandEntity, bool>>(x)!,
                ValueComparer.CreateDefault(typeof(Dictionary<DisabledCommandEntity, bool>), false));
        modelBuilder.Entity<ModMailGuildEntity>()
            .Property(x => x.Groups)
            .HasConversion(x => JsonConvert.SerializeObject(x),
                x => JsonConvert.DeserializeObject<Dictionary<DisabledGroupEntity, bool>>(x)!,
                ValueComparer.CreateDefault(typeof(Dictionary<DisabledGroupEntity, bool>), false));
        modelBuilder.Entity<ModMailGuildEntity>()
            .HasMany(x => x.ModMailThreads)
            .WithOne(x => x.GuildEntity)
            .HasForeignKey(x => x.GuildId);
    }

    public void Set(GuildEntity entity)
    {
        Cache.Remove(Cache.FirstOrDefault(x => x.GuildId == entity.GuildId)!);
        Cache.Add(ModMailGuildEntity.FromGuildEntity(entity));
        var guildEntity = Guilds.ToArray().FirstOrDefault(x => x.GuildId == entity.GuildId);
        if (guildEntity is not null)
            Guilds.Update(guildEntity!);
        else
            Guilds.Add(ModMailGuildEntity.FromGuildEntity(entity));
        SaveChanges();
    }

    public GuildEntity Get(DiscordGuild guild) =>
        Cache.FirstOrDefault(x => x.GuildId == guild.Id)!;

    public void Remove(GuildEntity entity)
    {
        if (Cache.All(x => x.GuildId != entity.GuildId)) return;
        Cache.RemoveAll(x => x.GuildId == entity.GuildId);
        var guildEntity = Guilds.Find(entity.GuildId);
        if (guildEntity is not null)
            Guilds.Remove(guildEntity!);
        SaveChanges();
    }

    public async void Init()
    {
        Cache.Clear();
        Cache.AddRange(Guilds.ToList());
    }
}