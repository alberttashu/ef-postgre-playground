// See https://aka.ms/new-console-template for more information

using Microsoft.EntityFrameworkCore;


Console.WriteLine("Hello, World!");

var context = new PlaygroundDbContext();

context.Database.EnsureDeleted();
context.Database.EnsureCreated();
context.Database.Migrate();


SeedData(context);

// Do stuff


Console.WriteLine("Ready.");

#region Setup

void SeedData(PlaygroundDbContext context)
{
    var user1 = new User(Guid.NewGuid(), "user1", new DateTime(2000, 01, 01).ToUniversalTime());
    var user2 = new User(Guid.NewGuid(), "user2", new DateTime(2000, 01, 02).ToUniversalTime());
    var user3 = new User(Guid.NewGuid(), "user3", new DateTime(2000, 01, 03).ToUniversalTime());

    var transactions1 = Enumerable.Range(1, 10)
        .Select(i => new Transaction(Guid.NewGuid(), new DateTime(2000, i, 01).ToUniversalTime(), i * 100, user1));
    var transactions2 = Enumerable.Range(1, 10)
        .Select(i => new Transaction(Guid.NewGuid(), new DateTime(2000, i, 01).ToUniversalTime(), i * 100, user2));
    var transactions3 = Enumerable.Range(1, 10)
        .Select(i => new Transaction(Guid.NewGuid(), new DateTime(2000, i, 01).ToUniversalTime(), i * 100, user3));

    context.Add(user1);
    context.Add(user2);
    context.Add(user3);

    var transactions = transactions1
        .Concat(transactions2)
        .Concat(transactions3)
        .ToList();

    context.AddRange(transactions);

    context.SaveChanges();
}


class User
{
    public Guid Id { get; private set; }
    public string Username { get; private set; }
    public DateTime Created { get; private set; }
    public ICollection<Transaction> Transactions { get; set; }

    public User()
    {
        Transactions = new List<Transaction>();
    }

    public User(Guid id, string username, DateTime created) : this()
    {
        Id = id;
        Username = username;
        Created = created;
    }
}

class Transaction
{
    public Guid Id { get; set; }
    public DateTime Created { get; set; }
    public int Amount { get; set; }
    public User User { get; set; }

    public Transaction()
    {
    }

    public Transaction(Guid id, DateTime created, int amount, User user)
    {
        Id = id;
        Created = created;
        Amount = amount;
        User = user;
    }
}

class PlaygroundDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Transaction> Transactions { get; set; }

    public PlaygroundDbContext(DbContextOptions<PlaygroundDbContext> options) : base(options)
    {
    }

    public PlaygroundDbContext()
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseNpgsql(
            "User ID=user;Password=password;Host=localhost;Port=5432;Database=playgroung_database;");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasMany(x => x.Transactions)
            .WithOne(x => x.User);

        modelBuilder.Entity<Transaction>()
            .HasOne(x => x.User)
            .WithMany(x => x.Transactions);
    }
}

#endregion