using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ExpenseClaim.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CostCenter",
                columns: table => new
                {
                    CostCenterId = table.Column<string>(nullable: false),
                    CostCenterName = table.Column<string>(maxLength: 50, nullable: false),
                    IsDefault = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CostCenter", x => x.CostCenterId);
                });

            migrationBuilder.CreateTable(
                name: "Expenses",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CostCenterId = table.Column<string>(nullable: true),
                    TotalAmount = table.Column<decimal>(nullable: false),
                    Gst = table.Column<decimal>(nullable: false),
                    TransactionDate = table.Column<DateTime>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    Vendor = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    PaymentMethod = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Expenses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Expenses_CostCenter_CostCenterId",
                        column: x => x.CostCenterId,
                        principalTable: "CostCenter",
                        principalColumn: "CostCenterId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_CostCenterId",
                table: "Expenses",
                column: "CostCenterId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Expenses");

            migrationBuilder.DropTable(
                name: "CostCenter");
        }
    }
}
