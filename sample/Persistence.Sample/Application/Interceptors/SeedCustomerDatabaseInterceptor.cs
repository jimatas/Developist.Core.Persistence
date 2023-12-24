using Developist.Core.Cqrs;
using Developist.Core.Persistence;
using Developist.Customers.Application.Utilities;
using Developist.Customers.Domain.Model;
using Microsoft.Extensions.Options;

namespace Developist.Customers.Application.Interceptors;

/// <summary>
/// Represents an interceptor for seeding the customer database if it is empty when processing messages of type <typeparamref name="TQuery"/>.
/// </summary>
/// <typeparam name="TQuery">The query type.</typeparam>
/// <typeparam name="TResult">The result type.</typeparam>
public class SeedCustomerDatabaseInterceptor<TQuery, TResult> : IQueryInterceptor<TQuery, TResult>
    where TQuery : IQuery<TResult>
{
    private readonly IUnitOfWorkBase _unitOfWork;
    private readonly ILogger _logger;
    private readonly AppSettings _appSettings;

    /// <summary>
    /// Initializes a new instance of the <see cref="SeedCustomerDatabaseInterceptor{T, TResult}"/> class.
    /// </summary>
    /// <param name="unitOfWork">The unit of work used for seeding the database.</param>
    /// <param name="logger">The logger used for logging messages.</param>
    /// <param name="appSettings">The application settings that control the database seeding process.</param>
    public SeedCustomerDatabaseInterceptor(
        IUnitOfWorkBase unitOfWork,
        ILogger<SeedCustomerDatabaseInterceptor<TQuery, TResult>> logger,
        IOptions<AppSettings> appSettings)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _appSettings = appSettings.Value;
    }

    /// <inheritdoc/>
    public async Task<TResult> InterceptAsync(TQuery query, QueryHandlerDelegate<TQuery, TResult> next, CancellationToken cancellationToken)
    {
        if (_appSettings.SeedDatabase)
        {
            try
            {
                if (!await _unitOfWork.Repository<Customer>().AnyAsync(cancellationToken))
                {
                    SeedDatabase(_appSettings.NumberOfCustomers, _appSettings.BaseCustomerNumber);
                    await _unitOfWork.CompleteAsync(cancellationToken);

                    _logger.LogInformation("Customers database seeded.");
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error while seeding customers database: {ExceptionMessage}", exception.Message);
            }
        }

        return await next(query, cancellationToken);
    }

    private void SeedDatabase(int numberOfCustomers, int baseCustomerNumber)
    {
        var random = new Random();

        for (var i = 1; i <= numberOfCustomers; i++)
        {
            var paymentInformation = CreatePaymentInformationForMethod(GetRandomPaymentMethod(random));
            var customerNumber = baseCustomerNumber + i;
            var customer = new Customer(customerNumber, paymentInformation);

            _logger.LogDebug("Adding {Customer} to customers database.", customer);
            _unitOfWork.Repository<Customer>().Add(customer);
        }
    }

    private static PaymentMethod GetRandomPaymentMethod(Random random)
    {
        var allPaymentMethods = Enum.GetValues<PaymentMethod>();
        return allPaymentMethods[random.Next(allPaymentMethods.Length)];
    }

    private static PaymentInformation CreatePaymentInformationForMethod(PaymentMethod paymentMethod)
    {
        return paymentMethod switch
        {
            _ when paymentMethod.IsBankPaymentMethod() => new BankPaymentInformation(paymentMethod, IbanGenerator.GenerateRandomIban()),
            _ when paymentMethod.IsCashPaymentMethod() => new CashPaymentInformation(paymentMethod),
            _ => new CreditCardPaymentInformation(CreditCardNumberGenerator.GenerateRandomCreditCardNumber())
        };
    }
}
