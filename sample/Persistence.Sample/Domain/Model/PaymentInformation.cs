namespace Developist.Customers.Domain.Model;

/// <summary>
/// Encapsulates the payment details of individual customers.
/// </summary>
public abstract class PaymentInformation
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PaymentInformation"/> class with the specified payment method.
    /// </summary>
    /// <param name="paymentMethod">The payment method associated with this payment information.</param>
    protected PaymentInformation(PaymentMethod paymentMethod) => PaymentMethod = paymentMethod;

    /// <summary>
    /// Gets the unique identifier for the payment information.
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Gets the payment method.
    /// </summary>
    public PaymentMethod PaymentMethod { get; }
}
