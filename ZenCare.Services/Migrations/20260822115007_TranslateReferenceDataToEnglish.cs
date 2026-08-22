using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZenCare.Services.Migrations
{
    /// <inheritdoc />
    public partial class TranslateReferenceDataToEnglish : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "FAQCategories",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Appointments");

            migrationBuilder.UpdateData(
                table: "FAQCategories",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "Payments");

            migrationBuilder.UpdateData(
                table: "FAQCategories",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "Products");

            migrationBuilder.UpdateData(
                table: "FAQCategories",
                keyColumn: "Id",
                keyValue: 4,
                column: "Name",
                value: "Wellness Services");

            migrationBuilder.UpdateData(
                table: "FAQCategories",
                keyColumn: "Id",
                keyValue: 5,
                column: "Name",
                value: "User Account");

            migrationBuilder.UpdateData(
                table: "ProductCategories",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Essential Oils");

            migrationBuilder.UpdateData(
                table: "ProductCategories",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "Skin Care");

            migrationBuilder.UpdateData(
                table: "ProductCategories",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "Scrubs");

            migrationBuilder.UpdateData(
                table: "ProductCategories",
                keyColumn: "Id",
                keyValue: 4,
                column: "Name",
                value: "Wellness Products");

            migrationBuilder.UpdateData(
                table: "ProductCategories",
                keyColumn: "Id",
                keyValue: 5,
                column: "Name",
                value: "Gift Sets");

            migrationBuilder.UpdateData(
                table: "ProductTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Product");

            migrationBuilder.UpdateData(
                table: "ProductTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "Oil");

            migrationBuilder.UpdateData(
                table: "ProductTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "Cream");

            migrationBuilder.UpdateData(
                table: "ProductTypes",
                keyColumn: "Id",
                keyValue: 4,
                column: "Name",
                value: "Scrub");

            migrationBuilder.UpdateData(
                table: "ProductTypes",
                keyColumn: "Id",
                keyValue: 5,
                column: "Name",
                value: "Set");

            migrationBuilder.UpdateData(
                table: "ServiceCategories",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Massages");

            migrationBuilder.UpdateData(
                table: "ServiceCategories",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "Aromatherapy");

            migrationBuilder.UpdateData(
                table: "ServiceCategories",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "Facial Care");

            migrationBuilder.UpdateData(
                table: "ServiceCategories",
                keyColumn: "Id",
                keyValue: 4,
                column: "Name",
                value: "Wellness Treatments");

            migrationBuilder.UpdateData(
                table: "ServiceCategories",
                keyColumn: "Id",
                keyValue: 5,
                column: "Name",
                value: "Relaxation");

            migrationBuilder.UpdateData(
                table: "UnitOfMeasures",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Abbreviation", "Name" },
                values: new object[] { "pc", "Piece" });

            migrationBuilder.UpdateData(
                table: "UnitOfMeasures",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Abbreviation", "Name" },
                values: new object[] { "pkg", "Package" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "FAQCategories",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Rezervacije");

            migrationBuilder.UpdateData(
                table: "FAQCategories",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "Plaćanje");

            migrationBuilder.UpdateData(
                table: "FAQCategories",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "Preparati");

            migrationBuilder.UpdateData(
                table: "FAQCategories",
                keyColumn: "Id",
                keyValue: 4,
                column: "Name",
                value: "Wellness usluge");

            migrationBuilder.UpdateData(
                table: "FAQCategories",
                keyColumn: "Id",
                keyValue: 5,
                column: "Name",
                value: "Korisnički račun");

            migrationBuilder.UpdateData(
                table: "ProductCategories",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Eterična ulja");

            migrationBuilder.UpdateData(
                table: "ProductCategories",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "Njega kože");

            migrationBuilder.UpdateData(
                table: "ProductCategories",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "Pilinzi");

            migrationBuilder.UpdateData(
                table: "ProductCategories",
                keyColumn: "Id",
                keyValue: 4,
                column: "Name",
                value: "Wellness preparati");

            migrationBuilder.UpdateData(
                table: "ProductCategories",
                keyColumn: "Id",
                keyValue: 5,
                column: "Name",
                value: "Poklon paketi");

            migrationBuilder.UpdateData(
                table: "ProductTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Preparat");

            migrationBuilder.UpdateData(
                table: "ProductTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "Ulje");

            migrationBuilder.UpdateData(
                table: "ProductTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "Krema");

            migrationBuilder.UpdateData(
                table: "ProductTypes",
                keyColumn: "Id",
                keyValue: 4,
                column: "Name",
                value: "Piling");

            migrationBuilder.UpdateData(
                table: "ProductTypes",
                keyColumn: "Id",
                keyValue: 5,
                column: "Name",
                value: "Paket");

            migrationBuilder.UpdateData(
                table: "ServiceCategories",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Masaže");

            migrationBuilder.UpdateData(
                table: "ServiceCategories",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "Aromaterapija");

            migrationBuilder.UpdateData(
                table: "ServiceCategories",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "Njega lica");

            migrationBuilder.UpdateData(
                table: "ServiceCategories",
                keyColumn: "Id",
                keyValue: 4,
                column: "Name",
                value: "Wellness tretmani");

            migrationBuilder.UpdateData(
                table: "ServiceCategories",
                keyColumn: "Id",
                keyValue: 5,
                column: "Name",
                value: "Relaksacija");

            migrationBuilder.UpdateData(
                table: "UnitOfMeasures",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Abbreviation", "Name" },
                values: new object[] { "kom", "kom" });

            migrationBuilder.UpdateData(
                table: "UnitOfMeasures",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Abbreviation", "Name" },
                values: new object[] { "pakovanje", "pakovanje" });
        }
    }
}
