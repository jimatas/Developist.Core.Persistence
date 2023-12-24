using Developist.Core.Cqrs;
using Developist.Customers.Domain.Model;

namespace Developist.Customers.Domain.Commands;

/// <summary>
/// Represents a request to change the payment method for a customer.
/// </summary>
/// <param name="CustomerNumber">The customer number for the customer whose payment method is being updated.</param>
/// <param name="PaymentInformation">The new payment information to be applied to the customer.</param>
public record ChangePaymentMethod(
    int CustomerNumber,
    PaymentInformation PaymentInformation) : ICommand;
