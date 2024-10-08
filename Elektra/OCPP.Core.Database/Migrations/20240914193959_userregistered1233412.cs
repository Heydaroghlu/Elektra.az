using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OCPP.Core.Database.Migrations
{
    /// <inheritdoc />
    public partial class userregistered1233412 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "PriceForReserv",
                table: "Tarifs",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<decimal>(
                name: "PriceForReserv2",
                table: "Tarifs",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceForStartSeans",
                table: "Tarifs",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceForStartSession",
                table: "Tarifs",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "Reserv",
                table: "Tarifs",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PriceForReserv2",
                table: "Tarifs");

            migrationBuilder.DropColumn(
                name: "PriceForStartSeans",
                table: "Tarifs");

            migrationBuilder.DropColumn(
                name: "PriceForStartSession",
                table: "Tarifs");

            migrationBuilder.DropColumn(
                name: "Reserv",
                table: "Tarifs");

            migrationBuilder.AlterColumn<decimal>(
                name: "PriceForReserv",
                table: "Tarifs",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);
        }
    }
}
