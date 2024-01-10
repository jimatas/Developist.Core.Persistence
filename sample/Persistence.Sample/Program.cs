using Developist.Core.Cqrs;
using Developist.Core.Persistence;
using Developist.Customers.Api.Model;
using Developist.Customers.Api.Serialization;
using Developist.Customers.Application;
using Developist.Customers.Domain.Commands;
using Developist.Customers.Domain.Model;
using Developist.Customers.Domain.Queries;
using Developist.Customers.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using HttpJsonOptions = Microsoft.AspNetCore.Http.Json.JsonOptions;
using MvcJsonOptions = Microsoft.AspNetCore.Mvc.JsonOptions;

var builder = WebApplication.CreateBuilder(args);

ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

ConfigureApplication(app);

// Paginates the list of customers using a specific payment method.
app.MapGet("/customers", async (IDispatcher mediator,
    [FromQuery] PaymentMethodsModel? paymentMethods,
    string? sortedBy,
    int? pageNumber,
    int? pageSize) =>
{
    var result = await mediator.DispatchAsync(new GetCustomers(paymentMethods?.ToArray())
    {
        PageNumber = pageNumber ?? 1,
        PageSize = pageSize ?? PaginationCriteria<Customer>.DefaultPageSize,
        SortedBy = sortedBy ?? nameof(Customer.CustomerNumber)
    });

    return Results.Ok(result);
});

// Changes the payment method for a customer.
// For example, new ChangePaymentMethod(5011, new BankPaymentInformation(PaymentMethod.Ideal, "NL91ABNA0417164300"));
app.MapPut("/customers/{number}", async (IDispatcher mediator,
    [FromRoute(Name = "number")] int customerNumber,
    PaymentInformationModel paymentInformation) =>
{
    await mediator.DispatchAsync(new ChangePaymentMethod(customerNumber, paymentInformation.ToPaymentInformation()));
});

app.Run();

#region ConfigureServices
static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    services.Configure<AppSettings>(configuration.GetSection(nameof(AppSettings)));
    services.AddLogging(logging => logging.AddConsole());

    services.AddDbContext<CustomersDbContext>(options =>
    {
        options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")
            ?? CustomersDbContext.LocalDbConnection);
    });

    services.AddUnitOfWork<CustomersDbContext>();

    // The following registrations enable the injection of Customer and PaymentInformation repositories as direct dependencies.
    services.AddScoped(provider => provider.GetRequiredService<IUnitOfWorkBase>().Repository<Customer>());
    services.AddScoped(provider => provider.GetRequiredService<IUnitOfWorkBase>().Repository<PaymentInformation>());

    services.AddCqrs(cfg =>
    {
        cfg.AddDefaultDispatcher();
        cfg.AddHandlersFromAssembly(typeof(Program).Assembly);
    });

    services.Configure<HttpJsonOptions>(options =>
    {
        options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.SerializerOptions.Converters.Add(new PaginatedListJsonConverter());
    });
    services.Configure<MvcJsonOptions>(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.Converters.Add(new PaginatedListJsonConverter());
    });

    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo { Title = "Developist.Customers.Api", Version = "v1" });

        var xmlFilePath = Path.Combine(AppContext.BaseDirectory, "Developist.Infrastructure.Persistence.Sample.xml");
        if (File.Exists(xmlFilePath))
        {
            options.IncludeXmlComments(xmlFilePath, includeControllerXmlComments: true);
        }

        options.DescribeAllParametersInCamelCase();
        options.MapType<PaymentMethodsModel>(() => new OpenApiSchema { Type = "string", Format = "comma-separated" });
    });
}
#endregion

#region ConfigureApplication
static void ConfigureApplication(WebApplication app)
{
    app.UseSwagger();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("v1/swagger.json", "Developist.Customers.Api v1"));

    app.UseHttpsRedirection();
}
#endregion
