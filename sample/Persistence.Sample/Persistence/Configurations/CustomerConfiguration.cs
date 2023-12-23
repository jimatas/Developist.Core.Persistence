using Developist.Customers.Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Developist.Customers.Persistence.Configurations;

internal class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> entity)
    {
        entity.ToTable(nameof(Customer));
        entity.HasKey(e => e.Id);
        entity.HasIndex(e => e.CustomerNumber).IsUnique();
    }
}
