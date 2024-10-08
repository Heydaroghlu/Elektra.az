using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OCPP.Core.Database.Migrations
{
    /// <inheritdoc />
    public partial class tarifsadd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.RenameColumn(
                name: "PowerKw",
                table: "ChargePoint",
                newName: "TarifId");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "ConnectorStatus",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Tarifs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PowerKw = table.Column<int>(type: "int", nullable: false),
                    PriceForKw = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PriceForHour = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PriceForReserv = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tarifs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChargePoint_TarifId",
                table: "ChargePoint",
                column: "TarifId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChargePoint_Tarifs_TarifId",
                table: "ChargePoint",
                column: "TarifId",
                principalTable: "Tarifs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChargePoint_Tarifs_TarifId",
                table: "ChargePoint");

            migrationBuilder.DropTable(
                name: "Tarifs");

            migrationBuilder.DropIndex(
                name: "IX_ChargePoint_TarifId",
                table: "ChargePoint");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "ConnectorStatus");

            migrationBuilder.RenameColumn(
                name: "TarifId",
                table: "ChargePoint",
                newName: "PowerKw");

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
    }
}
