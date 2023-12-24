namespace Developist.Customers.Domain.Model;

/// <summary>
/// Extension methods for the <see cref="PaymentMethod"/> enum.
/// </summary>
public static class PaymentMethodExtensions
{
    /// <summary>
    /// Checks if the payment method is a bank-related payment method.
    /// </summary>
    /// <param name="paymentMethod">The payment method to check.</param>
    /// <returns><see langword="true"/> if the payment method is a bank-related method; otherwise, <see langword="false"/>.</returns>
    public static bool IsBankPaymentMethod(this PaymentMethod paymentMethod)
    {
        return paymentMethod is PaymentMethod.Ideal or PaymentMethod.DirectDebit;
    }

    /// <summary>
    /// Checks if the payment method is a cash-related payment method.
    /// </summary>
    /// <param name="paymentMethod">The payment method to check.</param>
    /// <returns><see langword="true"/>  if the payment method is a cash-related method; otherwise, <see langword="false"/>.</returns>
    public static bool IsCashPaymentMethod(this PaymentMethod paymentMethod)
    {
        return paymentMethod is PaymentMethod.Cash or PaymentMethod.CashWithoutCosts;
    }
}
