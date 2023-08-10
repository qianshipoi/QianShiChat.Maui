namespace QianShiChatClient.Data;

public class ChatDbContext : DbContext, IChatContext
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
        modelBuilder.Entity<UserInfo>()
            .Property(p => p.CreateTime)
            .IsRequired();

        modelBuilder.Entity<ChatMessage>()
            .HasKey(p => p.LocalId);
        modelBuilder.Entity<ChatMessage>()
            .Property(p => p.LocalId)
            .ValueGeneratedOnAdd();
        modelBuilder.Entity<ChatMessage>()
            .HasIndex(p => p.Id)
            .IsUnique();

        modelBuilder.Entity<Session>()
            .HasKey(p => p.Id);
        modelBuilder.Entity<Session>()
            .Property(p => p.Id)
            .ValueGeneratedOnAdd();
    }

    private IDbContextTransaction _currentTransaction = default!;

    public IDbContextTransaction GetCurrentTransaction() => _currentTransaction;

    public bool HasActiveTransaction => _currentTransaction != null;

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        if (_currentTransaction != null) return null!;

        _currentTransaction = await Database.BeginTransactionAsync();

        return _currentTransaction;
    }

    public async Task CommitAsync(IDbContextTransaction transaction)
    {
        if (transaction == null) throw new ArgumentNullException(nameof(transaction));
        if (transaction != _currentTransaction) throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current");

        try
        {
            await SaveChangesAsync();
            transaction.Commit();
        }
        catch
        {
            RollbackTransaction();
            throw;
        }
        finally
        {
            if (_currentTransaction != null)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null!;
            }
        }
    }
    private void RollbackTransaction()
    {
        try
        {
            _currentTransaction?.Rollback();
        }
        finally
        {
            if (_currentTransaction != null)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null!;
            }
        }
    }
}
