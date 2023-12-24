namespace Developist.Customers.Application;

/// <summary>
/// Contains application settings.
/// </summary>
public class AppSettings
{
    /// <summary>
    /// Indicates whether the database should be seeded with initial data.
    /// </summary>
    public bool SeedDatabase { get; set; }

    /// <summary>
    /// The number of customer records to create when seeding the database.
    /// </summary>
    public int NumberOfCustomers { get; set; }

    /// <summary>
    /// The base number from which customer numbers will be incremented. 
    /// </summary>
    public int BaseCustomerNumber { get; set; }
}
