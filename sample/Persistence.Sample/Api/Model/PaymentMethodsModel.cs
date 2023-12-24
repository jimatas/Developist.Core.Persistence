using Developist.Customers.Domain.Model;

namespace Developist.Customers.Api.Model;

/// <summary>
/// DTO for enumerating <see cref="PaymentMethod"/> enum values in the API layer.
/// </summary>
public class PaymentMethodsModel
{
    private readonly PaymentMethod[] _values;

    /// <summary>
    /// Initializes a new instance of the <see cref="PaymentMethodsModel"/> class with the specified payment methods.
    /// </summary>
    /// <param name="values">An array of <see cref="PaymentMethod"/> enum values.</param>
    public PaymentMethodsModel(PaymentMethod[] values) => _values = values;

    /// <summary>
    /// Tries to parse a string to create a <see cref="PaymentMethodsModel"/> instance.
    /// </summary>
    /// <param name="value">The string to parse.</param>
    /// <param name="result">When this method returns, contains the <see cref="PaymentMethodsModel"/> instance if the parse succeeded, or <see langword="null"/> if the parse failed.
    /// The parse fails if the value does not contain valid payment method names separated by commas.</param>
    /// <returns><see langword="true"/> if value was parsed successfully; otherwise, <see langword="false"/>.</returns>
    public static bool TryParse(string value, out PaymentMethodsModel? result)
    {
        result = null;
        var list = new List<PaymentMethod>();

        foreach (var s in value.Split(',', StringSplitOptions.TrimEntries))
        {
            if (Enum.TryParse<PaymentMethod>(s, ignoreCase: true, out var e))
            {
                list.Add(e);
            }
            else
            {
                return false;
            }
        }

        result = new PaymentMethodsModel(list.ToArray());
        return true;
    }

    /// <summary>
    /// Returns an array of <see cref="PaymentMethod"/> enum values.
    /// </summary>
    /// <returns>An array of <see cref="PaymentMethod"/> enum values.</returns>
    public PaymentMethod[] ToArray() => _values;
}
