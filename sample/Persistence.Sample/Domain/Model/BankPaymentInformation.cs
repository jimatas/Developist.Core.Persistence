namespace Developist.Customers.Domain.Model;

/// <summary>
/// Represents payment information for bank payments.
/// </summary>
public class BankPaymentInformation : PaymentInformation
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BankPaymentInformation"/> class with the specified payment method and bank account number.
    /// </summary>
    /// <param name="paymentMethod">The payment method associated with this bank payment information.
    /// Should be either <see cref="PaymentMethod.Ideal"/> or <see cref="PaymentMethod.DirectDebit"/>.</param>
    /// <param name="bankAccountNumber">The bank account number used for bank payments.</param>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentOutOfRangeException"/>
    public BankPaymentInformation(PaymentMethod paymentMethod, string bankAccountNumber)
        : base(EnsureBankPaymentMethod(paymentMethod))
    {
        if (string.IsNullOrWhiteSpace(bankAccountNumber))
        {
            throw new ArgumentException("Bank account number cannot be empty.", nameof(bankAccountNumber));
        }
        
        BankAccountNumber = bankAccountNumber;
    }

    /// <summary>
    /// Gets the bank account number used for bank payments.
    /// </summary>
    public string BankAccountNumber { get; }

    private static PaymentMethod EnsureBankPaymentMethod(PaymentMethod paymentMethod)
    {
        if (!paymentMethod.IsBankPaymentMethod())
        {
            throw new ArgumentOutOfRangeException(
                paramName: nameof(paymentMethod),
                actualValue: paymentMethod,
                message: $"Invalid payment method; it should be either {PaymentMethod.Ideal} or {PaymentMethod.DirectDebit}.");
        }

        return paymentMethod;
    }
}
