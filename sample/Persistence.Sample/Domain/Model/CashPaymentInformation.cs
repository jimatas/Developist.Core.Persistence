namespace Developist.Customers.Domain.Model;

/// <summary>
/// Represents payment information for cash payments.
/// </summary>
public class CashPaymentInformation : PaymentInformation
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CashPaymentInformation"/> class with the specified payment method.
    /// </summary>
    /// <param name="paymentMethod">The payment method associated with this cash payment information.
    /// Should be either <see cref="PaymentMethod.Cash"/> or <see cref="PaymentMethod.CashWithoutCosts"/>.</param>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentOutOfRangeException"/>
    public CashPaymentInformation(PaymentMethod paymentMethod)
        : base(EnsureCashPaymentMethod(paymentMethod))
    {
    }

    private static PaymentMethod EnsureCashPaymentMethod(PaymentMethod paymentMethod)
    {
        if (!paymentMethod.IsCashPaymentMethod())
        {
            throw new ArgumentOutOfRangeException(
                paramName: nameof(paymentMethod),
                actualValue: paymentMethod,
                message: $"Invalid payment method; it should be either {PaymentMethod.Cash} or {PaymentMethod.CashWithoutCosts}.");
        }

        return paymentMethod;
    }
}
