using Developist.Core.Cqrs;
using Developist.Core.Persistence;
using Developist.Core.Persistence.EntityFrameworkCore;
using Developist.Customers.Domain.Commands;
using Developist.Customers.Domain.Model;

namespace Developist.Customers.Application.Commands;

/// <summary>
/// Represents a handler for processing <see cref="ChangePaymentMethod"/> requests.
/// </summary>
public class ChangePaymentMethodHandler : ICommandHandler<ChangePaymentMethod>
{
    private readonly IUnitOfWorkBase _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChangePaymentMethodHandler"/> class.
    /// </summary>
    /// <param name="unitOfWork">The unit of work used to update customer data.</param>
    public ChangePaymentMethodHandler(IUnitOfWorkBase unitOfWork) => _unitOfWork = unitOfWork;

    /// <inheritdoc/>
    public async Task HandleAsync(ChangePaymentMethod command, CancellationToken cancellationToken)
    {
        var customer = await GetCustomerByCustomerNumber(command.CustomerNumber, cancellationToken);
        UpdatePaymentInformation(customer, command.PaymentInformation);

        await _unitOfWork.CompleteAsync(cancellationToken);
    }

    private async Task<Customer> GetCustomerByCustomerNumber(int customerNumber, CancellationToken cancellationToken)
    {
        var customer = await _unitOfWork.Repository<Customer>()
            .WithIncludes(props => props.Include(c => c.PaymentInformation))
            .SingleOrDefaultAsync(c => c.CustomerNumber == customerNumber, cancellationToken);

        return customer ?? throw new KeyNotFoundException($"Customer with customer number {customerNumber} not found.");
    }

    private void UpdatePaymentInformation(Customer customer, PaymentInformation newPaymentInformation)
    {
        // If the customer already has payment information, remove it to replace it with the new information.
        if (customer.PaymentInformation is { } oldPaymentInformation)
        {
            _unitOfWork.Repository<PaymentInformation>().Remove(oldPaymentInformation);
        }

        customer.PaymentInformation = newPaymentInformation;
    }
}
