namespace Developist.Customers.Domain.Model;

/// <summary>
/// Represents various payment methods as an enum.
/// </summary>
public enum PaymentMethod
{
    /// <summary>
    /// Cash payment method.
    /// </summary>
    Cash = 1,

    /// <summary>
    /// Cash payment method without additional costs.
    /// </summary>
    CashWithoutCosts,

    /// <summary>
    /// Credit card payment method.
    /// </summary>
    CreditCard,

    /// <summary>
    /// Direct debit payment method.
    /// </summary>
    DirectDebit,

    /// <summary>
    /// iDEAL payment method.
    /// </summary>
    Ideal
}
