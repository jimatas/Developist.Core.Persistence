using Developist.Core.Persistence;
using Developist.Customers.Domain.Model;

namespace Developist.Customers.Application.Queries.Criteria;

/// <summary>
/// Represents filtering criteria specifically for <see cref="Customer"/> entities.
/// </summary>
public class CustomerCriteria : IFilterCriteria<Customer>
{
    /// <summary>
    /// Gets or sets a collection of <see cref="PaymentMethod"/> values used to filter <see cref="Customer"/> entities.
    /// </summary>
    public ICollection<PaymentMethod>? PaymentMethods { get; set; }

    /// <inheritdoc/>
    public IQueryable<Customer> Apply(IQueryable<Customer> query)
    {
        if (PaymentMethods?.Any() is true)
        {
            query = query.Where(cust => PaymentMethods.Contains(cust.PaymentInformation!.PaymentMethod));
        }

        return query;
    }
}
