using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OCPP.Core.Database.Migrations
{
    /// <inheritdoc />
    public partial class CpImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ChargeLocations_ChargePointId",
                table: "ChargeLocations");

            migrationBuilder.CreateTable(
                name: "CpImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsPoster = table.Column<bool>(type: "bit", nullable: false),
                    ChargePointId = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CpImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CpImages_ChargePoint_ChargePointId",
                        column: x => x.ChargePointId,
                        principalTable: "ChargePoint",
                        principalColumn: "ChargePointId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChargeLocations_ChargePointId",
                table: "ChargeLocations",
                column: "ChargePointId",
                unique: true,
                filter: "[ChargePointId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CpImages_ChargePointId",
                table: "CpImages",
                column: "ChargePointId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CpImages");

            migrationBuilder.DropIndex(
                name: "IX_ChargeLocations_ChargePointId",
                table: "ChargeLocations");

            migrationBuilder.CreateIndex(
                name: "IX_ChargeLocations_ChargePointId",
                table: "ChargeLocations",
                column: "ChargePointId");
        }
    }
}
