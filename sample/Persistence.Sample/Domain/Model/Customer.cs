namespace Developist.Customers.Domain.Model;

/// <summary>
/// Represents a customer entity.
/// </summary>
public class Customer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Customer"/> class.
    /// </summary>
    /// <param name="customerNumber">The customer number.</param>
    /// <exception cref="ArgumentOutOfRangeException"/>
    public Customer(int customerNumber)
    {
        CustomerNumber = customerNumber;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Customer"/> class.
    /// </summary>
    /// <param name="customerNumber">The customer number.</param>
    /// <param name="paymentInformation">The payment information for the customer.</param>
    /// <exception cref="ArgumentNullException"/>
    public Customer(int customerNumber, PaymentInformation paymentInformation)
        : this(customerNumber)
    {
        PaymentInformation = paymentInformation
            ?? throw new ArgumentNullException(nameof(paymentInformation));
    }

    /// <summary>
    /// Gets the unique identifier for the customer.
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Gets the customer number.
    /// </summary>
    public int CustomerNumber { get; }

    /// <summary>
    /// Gets or sets the payment information for the customer.
    /// </summary>
    public PaymentInformation? PaymentInformation { get; set; }

    /// <summary>
    /// Returns a string representation of the customer.
    /// </summary>
    /// <returns>A string containing customer information.</returns>
    public override string ToString()
    {
        return $"Customer with customer number {CustomerNumber} "
            + $"and payment method '{PaymentInformation?.PaymentMethod.ToString() ?? "Unknown"}'.";
    }
}
