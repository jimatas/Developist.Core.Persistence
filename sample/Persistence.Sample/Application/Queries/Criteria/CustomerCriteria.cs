using Developist.Customers.Domain.Model;
using Developist.Customers.Persistence;

namespace Developist.Customers.Application.Queries.Criteria;

/// <summary>
/// Represents pagination and filtering criteria specifically for <see cref="Customer"/> entities.
/// </summary>
public class CustomerCriteria : PaginatedFilterCriteriaBase<Customer>
{
    /// <summary>
    /// Gets or sets a collection of <see cref="PaymentMethod"/> values used to filter <see cref="Customer"/> entities.
    /// </summary>
    public ICollection<PaymentMethod>? PaymentMethods { get; set; }

    /// <inheritdoc/>
    protected override IQueryable<Customer> ApplyFilter(IQueryable<Customer> query)
    {
        if (PaymentMethods?.Any() is true)
        {
            query = query.Where(cust => PaymentMethods.Contains(cust.PaymentInformation!.PaymentMethod));
        }

        return query;
    }
}
