using Microsoft.EntityFrameworkCore;

namespace Developist.Customers.Persistence;

/// <summary>
/// Represents the database context for customer data.
/// </summary>
public class CustomersDbContext : DbContext
{
    /// <summary>
    /// The default connection string for local development.
    /// </summary>
    public const string LocalDbConnection = @"Server=(localdb)\mssqllocaldb;Database=SampleCustomersDb;Trusted_Connection=true;MultipleActiveResultSets=true";

    /// <summary>
    /// Initialize a new instance of the <see cref="CustomersDbContext"/> class using the default constructor.
    /// </summary>
    public CustomersDbContext() { }

    /// <summary>
    /// Initialize a new instance of the <see cref="CustomersDbContext"/> class using the specified <see cref="DbContext"/> options.
    /// </summary>
    /// <param name="options">The options to be used by the <see cref="DbContext"/>.</param>
    public CustomersDbContext(DbContextOptions<CustomersDbContext> options) : base(options) { }

    #region DbContext overrides
    /// <inheritdoc/>
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        if (!options.IsConfigured)
        {
            options.UseSqlServer(LocalDbConnection);
        }

        base.OnConfiguring(options);
    }

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder model)
    {
        model.ApplyConfigurationsFromAssembly(GetType().Assembly);

        base.OnModelCreating(model);
    }
    #endregion
}
