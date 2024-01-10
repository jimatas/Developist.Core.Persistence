using Developist.Core.Cqrs;
using Developist.Core.Persistence;
using Developist.Customers.Domain.Model;

namespace Developist.Customers.Domain.Queries;

/// <summary>
/// Represents a request to retrieve a list of customers with optional filtering and pagination parameters.
/// </summary>
/// <param name="PaymentMethods">The optionally specified payment methods filter.</param>
/// <param name="SortedBy">A comma-separated list of property names to sort by. Defaults to <see cref="Customer.CustomerNumber"/> if not specified.</param>
/// <param name="PageNumber">The page number for pagination. Defaults to 1 if not specified.</param>
/// <param name="PageSize">The page size for pagination. Defaults to <see cref="PaginationCriteria{T}.DefaultPageSize"/>.</param>
public record GetCustomers(
    PaymentMethod[]? PaymentMethods = null,
    string SortedBy = nameof(Customer.CustomerNumber),
    int PageNumber = 1,
    int PageSize = PaginationCriteria<Customer>.DefaultPageSize) : IQuery<IPaginatedList<Customer>>;
