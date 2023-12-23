namespace Developist.Customers.Application.Utilities;

/// <summary>
/// Provides methods for generating random credit card numbers.
/// </summary>
/// <remarks>
/// Note that the generated credit card numbers are suitable only for testing purposes, not real-world financial scenarios.
/// </remarks>
public static class CreditCardNumberGenerator
{
    private static readonly string[] MasterCardPrefixes = { "51", "52", "53", "54", "55" };
    private static readonly Random Random = new();

    /// <summary>
    /// Generates a random credit card number within the 51-55 range of MasterCard IINs (Issuer Identification Numbers).
    /// </summary>
    /// <returns>A randomly generated credit card number.</returns>
    public static string GenerateRandomCreditCardNumber()
    {
        var prefix = GetRandomPrefix();
        var digits = GetRandomDigits(length: 13);
        var checkDigit = CalculateCheckDigit(prefix + digits);

        return $"{prefix}{digits}{checkDigit}";
    }

    private static string GetRandomPrefix()
    {
        return MasterCardPrefixes[Random.Next(MasterCardPrefixes.Length)];
    }

    private static string GetRandomDigits(int length)
    {
        return Random.NextInt64((long)Math.Pow(10, length - 1), (long)Math.Pow(10, length)).ToString();
    }

    private static int CalculateCheckDigit(string payload)
    {
        var sum = payload.Reverse().Select((c, i) => GetAdjustedDigit(c - '0', i)).Sum();
        return (10 - sum % 10) % 10;
    }

    private static int GetAdjustedDigit(int digit, int index)
    {
        if (index % 2 == 0)
        {
            digit *= 2;
            if (digit > 9)
            {
                digit -= 9;
            }
        }

        return digit;
    }
}
