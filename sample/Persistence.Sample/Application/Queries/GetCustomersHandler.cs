using Developist.Core.Cqrs;
using Developist.Core.Persistence;
using Developist.Core.Persistence.EntityFrameworkCore;
using Developist.Core.Persistence.Pagination;
using Developist.Customers.Application.Queries.Criteria;
using Developist.Customers.Domain.Model;
using Developist.Customers.Domain.Queries;

namespace Developist.Customers.Application.Queries;

/// <summary>
/// Represents a handler for processing <see cref="GetCustomers"/> requests.
/// </summary>
public class GetCustomersHandler : IQueryHandler<GetCustomers, IPaginatedList<Customer>>
{
    private readonly IRepository<Customer> _customerRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetCustomersHandler"/> class.
    /// </summary>
    /// <param name="repository">The repository used to retrieve customer data.</param>
    public GetCustomersHandler(IRepository<Customer> repository) => _customerRepository = repository;

    /// <inheritdoc/>
    public async Task<IPaginatedList<Customer>> HandleAsync(GetCustomers query, CancellationToken cancellationToken)
    {
        var result = await _customerRepository
            .WithIncludes(props => props.Include(c => c.PaymentInformation))
            .ListAsync(
                new CustomerCriteria { PaymentMethods = query.PaymentMethods },
                p => p.StartAtPage(query.PageNumber)
                      .UsePageSize(query.PageSize)
                      .SortByString(query.SortedBy),
                cancellationToken);

        return result;
    }
}
