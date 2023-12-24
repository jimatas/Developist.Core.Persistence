using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Developist.Customers.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BankPaymentInformation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    BankAccountNumber = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankPaymentInformation", x => x.Id);
                    table.CheckConstraint("CK_BankPaymentInformation_PaymentMethod", "[PaymentMethod] IN ('DirectDebit', 'Ideal')");
                });

            migrationBuilder.CreateTable(
                name: "CashPaymentInformation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CashPaymentInformation", x => x.Id);
                    table.CheckConstraint("CK_CashPaymentInformation_PaymentMethod", "[PaymentMethod] IN ('Cash', 'CashWithoutCosts')");
                });

            migrationBuilder.CreateTable(
                name: "CreditCardPaymentInformation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreditCardNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditCardPaymentInformation", x => x.Id);
                    table.CheckConstraint("CK_CreditCardPaymentInformation_PaymentMethod", "[PaymentMethod] IN ('CreditCard')");
                });

            migrationBuilder.CreateTable(
                name: "Customer",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerNumber = table.Column<int>(type: "int", nullable: false),
                    PaymentInformationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customer", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Customer_CustomerNumber",
                table: "Customer",
                column: "CustomerNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customer_PaymentInformationId",
                table: "Customer",
                column: "PaymentInformationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BankPaymentInformation");

            migrationBuilder.DropTable(
                name: "CashPaymentInformation");

            migrationBuilder.DropTable(
                name: "CreditCardPaymentInformation");

            migrationBuilder.DropTable(
                name: "Customer");
        }
    }
}
