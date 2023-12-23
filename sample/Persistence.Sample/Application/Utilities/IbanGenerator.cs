namespace Developist.Customers.Application.Utilities;

/// <summary>
/// Provides methods for generating random IBANs (International Bank Account Numbers).
/// </summary>
/// <remarks>
/// Note that the generated IBANs are suitable only for testing purposes, not real-world financial scenarios.
/// </remarks>
public static class IbanGenerator
{
    private const string CountryCode = "NL";
    private static readonly string[] BankCodes = { "RABO", "INGB", "ABNA" };
    private static readonly Random Random = new();

    /// <summary>
    /// Generates a random NL (Netherlands) IBAN.
    /// </summary>
    /// <returns>A randomly generated NL IBAN.</returns>
    public static string GenerateRandomIban()
    {
        var bankCode = GetRandomBankCode();
        var accountNumber = GetRandomDigits(length: 10);

        // IBAN with rearranged elements for checksum calculation: bank code and account number first, followed by the country code and placeholder check digits ("00").
        var checksum = CalculateChecksum($"{bankCode}{accountNumber}{CountryCode}00");
        var checkDigits = (98 - checksum).ToString("D2");

        return $"{CountryCode}{checkDigits}{bankCode}{accountNumber}";
    }

    private static string GetRandomBankCode()
    {
        return BankCodes[Random.Next(BankCodes.Length)];
    }

    private static string GetRandomDigits(int length)
    {
        return Random.NextInt64((long)Math.Pow(10, length - 1), (long)Math.Pow(10, length)).ToString();
    }

    private static int CalculateChecksum(string payload)
    {
        var digits = string.Concat(payload.Select(c => char.IsDigit(c) ? c : char.ToUpper(c) - 'A' + 10));

        var remainder = BigInteger.Parse(digits) % 97;
        return (int)remainder;
    }
}
