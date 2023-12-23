namespace Developist.Customers.Domain.Model;

/// <summary>
/// Represents payment information for credit card payments.
/// </summary>
public class CreditCardPaymentInformation : PaymentInformation
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreditCardPaymentInformation"/> class with the specified credit card number.
    /// </summary>
    /// <param name="creditCardNumber">The credit card number used for credit card payments.</param>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="ArgumentException"/>
    public CreditCardPaymentInformation(string creditCardNumber)
        : base(PaymentMethod.CreditCard)
    {
        if (string.IsNullOrWhiteSpace(creditCardNumber))
        {
            throw new ArgumentException("Credit card number cannot be empty.", nameof(creditCardNumber));
        }

        CreditCardNumber = creditCardNumber;
    }

    /// <summary>
    /// Gets the credit card number used for credit card payments.
    /// </summary>
    public string CreditCardNumber { get; }
}
