namespace Developist.Core.Persistence.Utilities;

/// <summary>
/// Provides a set of methods for argument validation and ensuring proper parameter values.
/// </summary>
internal static class Ensure
{
    /// <summary>
    /// Ensures that a value is not <see langword="null"/>.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="value">The value to check.</param>
    /// <param name="paramName">The name of the parameter.</param>
    /// <returns>The input value if it is not <see langword="null"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the input value is <see langword="null"/>.</exception>
    public static T NotNull<T>([NotNull] T? value,
        [CallerArgumentExpression("value")] string? paramName = default)
    {
        return value ?? throw new ArgumentNullException(paramName);
    }

    /// <summary>
    /// Ensures that a string is not <see langword="null"/> or whitespace.
    /// </summary>
    /// <param name="value">The string to check.</param>
    /// <param name="paramName">The name of the parameter.</param>
    /// <returns>The input string if it is not <see langword="null"/> or whitespace.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the input string is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown if the input string is empty or contains only whitespace characters.</exception>
    public static string NotNullOrWhiteSpace([NotNull] string? value,
        [CallerArgumentExpression("value")] string? paramName = default)
    {
        NotNull(value, paramName);

        if (value.Length == 0 || value.All(char.IsWhiteSpace))
        {
            throw new ArgumentException(
                message: "Value cannot be empty or contain only whitespace characters.",
                paramName);
        }

        return value;
    }

    /// <summary>
    /// Ensures that a value is not less than a specified minimum value.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="minValue">The minimum allowed value.</param>
    /// <param name="actualValue">The actual value to check.</param>
    /// <param name="paramName">The name of the parameter.</param>
    /// <returns>The actual value if it is not less than the minimum value.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the actual value is less than the minimum value.</exception>
    public static T NotLessThan<T>(T minValue, T actualValue,
        [CallerArgumentExpression("actualValue")] string? paramName = default) where T : IComparable<T>
    {
        if (actualValue.CompareTo(minValue) < 0)
        {
            throw new ArgumentOutOfRangeException(
                paramName,
                actualValue,
                message: $"Value cannot be less than {minValue}.");
        }

        return actualValue;
    }

    /// <summary>
    /// Ensures that an enum value is valid within its enumeration type.
    /// </summary>
    /// <typeparam name="T">The enumeration type.</typeparam>
    /// <param name="value">The enum value to check.</param>
    /// <param name="paramName">The name of the parameter.</param>
    /// <returns>The input enum value if it is valid within the enumeration type.</returns>
    /// <exception cref="InvalidEnumArgumentException">Thrown if the enum value is not defined in the enumeration type.</exception>
    public static T NotInvalidEnum<T>(T value,
        [CallerArgumentExpression("value")] string? paramName = default) where T : Enum
    {
        if (!Enum.IsDefined(typeof(T), value))
        {
            throw new InvalidEnumArgumentException(paramName,
                invalidValue: Convert.ToInt32(value),
                enumClass: typeof(T));
        }

        return value;
    }
}
