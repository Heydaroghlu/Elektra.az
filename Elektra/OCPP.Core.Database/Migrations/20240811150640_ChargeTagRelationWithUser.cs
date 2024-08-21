using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OCPP.Core.Database.Migrations
{
    /// <inheritdoc />
    public partial class ChargeTagRelationWithUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AppUserId",
                table: "ChargeTags",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChargeTags_AppUserId",
                table: "ChargeTags",
                column: "AppUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChargeTags_AspNetUsers_AppUserId",
                table: "ChargeTags",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChargeTags_AspNetUsers_AppUserId",
                table: "ChargeTags");

            migrationBuilder.DropIndex(
                name: "IX_ChargeTags_AppUserId",
                table: "ChargeTags");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "ChargeTags");
        }
    }
}
