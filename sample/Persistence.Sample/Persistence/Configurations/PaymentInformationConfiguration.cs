using Developist.Customers.Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Developist.Customers.Persistence.Configurations;

internal class PaymentInformationConfiguration :
    IEntityTypeConfiguration<PaymentInformation>,
    IEntityTypeConfiguration<BankPaymentInformation>,
    IEntityTypeConfiguration<CashPaymentInformation>,
    IEntityTypeConfiguration<CreditCardPaymentInformation>
{
    public void Configure(EntityTypeBuilder<PaymentInformation> entity)
    {
        entity.UseTpcMappingStrategy();
        entity.Property(e => e.PaymentMethod).HasConversion<string>().HasMaxLength(20);
        entity.HasKey(e => e.Id);
    }

    public void Configure(EntityTypeBuilder<BankPaymentInformation> entity)
    {
        var tableName = nameof(BankPaymentInformation);
        var paymentMethodColumnName = entity.Metadata.GetProperty(nameof(BankPaymentInformation.PaymentMethod)).GetColumnName();

        entity.ToTable(tableName, table => table.HasCheckConstraint(
            name: $"CK_{tableName}_{paymentMethodColumnName}",
            sql: $"[{paymentMethodColumnName}] IN ('{PaymentMethod.DirectDebit}', '{PaymentMethod.Ideal}')"));

        entity.Property(e => e.BankAccountNumber).HasMaxLength(40);
    }

    public void Configure(EntityTypeBuilder<CashPaymentInformation> entity)
    {
        var tableName = nameof(CashPaymentInformation);
        var paymentMethodColumnName = entity.Metadata.GetProperty(nameof(CashPaymentInformation.PaymentMethod)).GetColumnName();

        entity.ToTable(table => table.HasCheckConstraint(
            name: $"CK_{tableName}_{paymentMethodColumnName}",
            sql: $"[{paymentMethodColumnName}] IN ('{PaymentMethod.Cash}', '{PaymentMethod.CashWithoutCosts}')"));
    }

    public void Configure(EntityTypeBuilder<CreditCardPaymentInformation> entity)
    {
        var tableName = nameof(CreditCardPaymentInformation);
        var paymentMethodColumnName = entity.Metadata.GetProperty(nameof(CreditCardPaymentInformation.PaymentMethod)).GetColumnName();

        entity.ToTable(table => table.HasCheckConstraint(
            name: $"CK_{tableName}_{paymentMethodColumnName}",
            sql: $"[{paymentMethodColumnName}] IN ('{PaymentMethod.CreditCard}')"));

        entity.Property(e => e.CreditCardNumber).HasMaxLength(20);
    }
}
