using Microsoft.EntityFrameworkCore;

using QianShiChatClient.Core;

namespace QianShiChatClient.Data;

public class ChatDbContext : DbContext
{
    public ChatDbContext(DbContextOptions options) : base(options)
    {
        SQLitePCL.Batteries_V2.Init();

        Database.EnsureCreated();
    }

    public virtual DbSet<UserInfo> UserInfos { get; } = default!;

    public virtual DbSet<ChatMessage> ChatMessages { get; } = default!;

    public virtual DbSet<Session> Sessions { get; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserInfo>()
            .HasKey(p => p.Id);
        modelBuilder.Entity<UserInfo>()
            .HasIndex(p => p.Account)
            .IsUnique();
        modelBuilder.Entity<UserInfo>()
            .Property(p => p.Account)
            .IsRequired()
            .HasMaxLength(64);
        modelBuilder.Entity<UserInfo>()
            .Property(p => p.NickName)
            .IsRequired()
            .HasMaxLength(64);

        modelBuilder.Entity<ChatMessage>()
            .HasKey(p => p.LocalId);
        modelBuilder.Entity<ChatMessage>()
            .Property(p => p.LocalId)
            .ValueGeneratedOnAdd();
        modelBuilder.Entity<ChatMessage>()
            .HasIndex(p => p.Id)
            .IsUnique();
        modelBuilder.Entity<ChatMessage>()
            .Ignore(p => p.FromAvatar)
            .Ignore(p => p.ToAvatar);

        modelBuilder.Entity<Session>()
            .HasKey(p => p.Id);
    }
}
