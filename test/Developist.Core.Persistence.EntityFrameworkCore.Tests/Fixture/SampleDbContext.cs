using Developist.Core.Persistence.Tests.Fixture;
using Microsoft.EntityFrameworkCore;

namespace Developist.Core.Persistence.EntityFrameworkCore.Tests.Fixture;

public class SampleDbContext : DbContext
{
    public SampleDbContext() { }
    public SampleDbContext(DbContextOptions<SampleDbContext> options)
        : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        if (!options.IsConfigured)
        {
            options.UseInMemoryDatabase(databaseName: $"{nameof(SampleDbContext)}_{Guid.NewGuid():N}");
        }

        base.OnConfiguring(options);
    }

    protected override void OnModelCreating(ModelBuilder model)
    {
        model.Entity<Person>().HasKey(p => p.Id);
        model.Entity<Person>().Property(p => p.GivenName).HasMaxLength(50);
        model.Entity<Person>().Property(p => p.FamilyName).HasMaxLength(100);
        model.Entity<Person>().HasOne(p => p.FavoriteBook).WithMany();

        model.Entity<SocialPerson>();

        model.Entity<Book>().HasKey(b => b.Id);
        model.Entity<Book>().Property(b => b.Title).HasMaxLength(50);

        base.OnModelCreating(model);
    }
}
