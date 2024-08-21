using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OCPP.Core.Database.Migrations
{
    /// <inheritdoc />
    public partial class PricesAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChargePoint_PriceCharge_PriceChargeId",
                table: "ChargePoint");

            migrationBuilder.DropTable(
                name: "PriceCharge");

            migrationBuilder.DropIndex(
                name: "IX_ChargePoint_PriceChargeId",
                table: "ChargePoint");

            migrationBuilder.AddColumn<decimal>(
                name: "PriceForHour",
                table: "ChargePoint",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceForKw",
                table: "ChargePoint",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceForReserv",
                table: "ChargePoint",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PriceForHour",
                table: "ChargePoint");

            migrationBuilder.DropColumn(
                name: "PriceForKw",
                table: "ChargePoint");

            migrationBuilder.DropColumn(
                name: "PriceForReserv",
                table: "ChargePoint");

            migrationBuilder.CreateTable(
                name: "PriceCharge",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    PriceForHour = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PriceForKw = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PriceForReserv = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceCharge", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChargePoint_PriceChargeId",
                table: "ChargePoint",
                column: "PriceChargeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChargePoint_PriceCharge_PriceChargeId",
                table: "ChargePoint",
                column: "PriceChargeId",
                principalTable: "PriceCharge",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
