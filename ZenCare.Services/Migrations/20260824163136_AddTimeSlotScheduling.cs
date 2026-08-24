using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZenCare.Services.Migrations
{
    /// <inheritdoc />
    public partial class AddTimeSlotScheduling : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TimeSlotId",
                table: "Appointments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TimeSlots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    WellnessServiceId = table.Column<int>(type: "int", nullable: false),
                    SlotDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeSlots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TimeSlots_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TimeSlots_WellnessServices_WellnessServiceId",
                        column: x => x.WellnessServiceId,
                        principalTable: "WellnessServices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_TimeSlotId",
                table: "Appointments",
                column: "TimeSlotId",
                unique: true,
                filter: "[TimeSlotId] IS NOT NULL AND [Status] <> 5");

            migrationBuilder.CreateIndex(
                name: "IX_TimeSlots_EmployeeId_SlotDate_StartTime",
                table: "TimeSlots",
                columns: new[] { "EmployeeId", "SlotDate", "StartTime" });

            migrationBuilder.CreateIndex(
                name: "IX_TimeSlots_WellnessServiceId_SlotDate_StartTime",
                table: "TimeSlots",
                columns: new[] { "WellnessServiceId", "SlotDate", "StartTime" });

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_TimeSlots_TimeSlotId",
                table: "Appointments",
                column: "TimeSlotId",
                principalTable: "TimeSlots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_TimeSlots_TimeSlotId",
                table: "Appointments");

            migrationBuilder.DropTable(
                name: "TimeSlots");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_TimeSlotId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "TimeSlotId",
                table: "Appointments");
        }
    }
}
