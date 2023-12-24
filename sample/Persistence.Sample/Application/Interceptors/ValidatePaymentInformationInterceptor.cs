using Developist.Core.Cqrs;
using Developist.Customers.Domain.Commands;
using Developist.Customers.Domain.Model;

namespace Developist.Customers.Application.Interceptors;

/// <summary>
/// Represents an interceptor for validating payment information when processing <see cref="ChangePaymentMethod"/> requests.
/// </summary>
public class ValidatePaymentInformationInterceptor : ICommandInterceptor<ChangePaymentMethod>
{
    /// <inheritdoc/>
    public Task InterceptAsync(ChangePaymentMethod command, CommandHandlerDelegate<ChangePaymentMethod> next, CancellationToken cancellationToken)
    {
        ValidatePaymentInformation(command.PaymentInformation);

        return next(command, cancellationToken);
    }

    private static void ValidatePaymentInformation(PaymentInformation paymentInformation)
    {
        if (paymentInformation.PaymentMethod.IsBankPaymentMethod() && !IsBankInformationProvided(paymentInformation))
        {
            throw new ValidationException($"Bank account information is required for {paymentInformation.PaymentMethod} payment method.");
        }

        if (paymentInformation.PaymentMethod is PaymentMethod.CreditCard && !IsCreditCardInformationProvided(paymentInformation))
        {
            throw new ValidationException($"Credit card information is required for {paymentInformation.PaymentMethod} payment method.");
        }
    }

    private static bool IsBankInformationProvided(PaymentInformation paymentInformation)
    {
        return paymentInformation is BankPaymentInformation bankPaymentInformation
            && !string.IsNullOrEmpty(bankPaymentInformation.BankAccountNumber);
    }

    private static bool IsCreditCardInformationProvided(PaymentInformation paymentInformation)
    {
        return paymentInformation is CreditCardPaymentInformation creditCardPaymentInformation
            && !string.IsNullOrEmpty(creditCardPaymentInformation.CreditCardNumber);
    }
}
