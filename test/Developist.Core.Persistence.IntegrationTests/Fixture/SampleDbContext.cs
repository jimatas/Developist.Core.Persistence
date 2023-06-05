using Microsoft.EntityFrameworkCore;

namespace Developist.Core.Persistence.IntegrationTests.Fixture;

public class SampleDbContext : DbContext
{
    private const string DefaultConnectionString = $"Server=(localdb)\\mssqllocaldb;Database=Developist_Core_Persistence_IntegrationTests;Trusted_Connection=true;MultipleActiveResultSets=true";

    public SampleDbContext() { }
    public SampleDbContext(DbContextOptions<SampleDbContext> options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(DefaultConnectionString);
        }

        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Person>(entity =>
        {
            entity.ToTable("People");
            entity.HasKey(p => p.Id);
            entity.Property(p => p.GivenName).HasMaxLength(20);
            entity.Property(p => p.FamilyName).HasMaxLength(20);
            entity.HasMany(p => p.ReceivedMessages).WithMany(m => m.Recipients).UsingEntity<ReceivedMessage>(
                x => x.HasOne(y => y.Message).WithMany(),
                x => x.HasOne(y => y.Recipient).WithMany()
            ).ToTable("ReceivedMessages");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.ToTable("Messages");
            entity.HasKey(m => m.Id);
            entity.Property(m => m.Text).HasMaxLength(100);
            entity.HasOne(m => m.Sender).WithMany(u => u.SentMessages).HasForeignKey("SenderId");
        });

        base.OnModelCreating(modelBuilder);
    }
}
