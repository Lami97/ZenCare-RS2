using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZenCare.Services.Migrations
{
    /// <inheritdoc />
    public partial class AddFAQDisplayOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DisplayOrder",
                table: "FAQs",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisplayOrder",
                table: "FAQs");
        }
    }
}
