using Developist.Customers.Domain.Model;

namespace Developist.Customers.Api.Model;

/// <summary>
/// DTO for the customer's payment information in the API layer.
/// </summary>
public class PaymentInformationModel
{
    /// <summary>
    /// Gets or sets the payment method, defined by the <see cref="Domain.Model.PaymentMethod"/> enum.
    /// </summary>
    public PaymentMethod PaymentMethod { get; set; }

    /// <summary>
    /// Gets or sets the payment details.
    /// The content depends on the payment method: it can be <see langword="null"/>, a bank account number, or a credit card number.
    /// </summary>
    /// <example>NL91ABNA0417164300</example>
    public string? PaymentDetails { get; set; }

    /// <summary>
    /// Converts this model to a domain-specific <see cref="PaymentInformation"/> object, based on the payment method.
    /// </summary>
    /// <returns>A <see cref="PaymentInformation"/> object corresponding to the specified payment method and details.</returns>
    public PaymentInformation ToPaymentInformation()
    {
        return PaymentMethod switch
        {
            PaymentMethod.DirectDebit or PaymentMethod.Ideal => new BankPaymentInformation(PaymentMethod, PaymentDetails!),
            PaymentMethod.Cash or PaymentMethod.CashWithoutCosts => new CashPaymentInformation(PaymentMethod),
            PaymentMethod.CreditCard => new CreditCardPaymentInformation(PaymentDetails!),
            _ => throw new NotSupportedException($"Unsupported payment method: {PaymentMethod}")
        };
    }
}
