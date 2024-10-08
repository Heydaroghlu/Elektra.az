using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OCPP.Core.Database.Migrations
{
    /// <inheritdoc />
    public partial class userregistered12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ChargeTags_AppUserId",
                table: "ChargeTags");

            migrationBuilder.AddColumn<DateTime>(
                name: "RegisteredTime",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_ChargeTags_AppUserId",
                table: "ChargeTags",
                column: "AppUserId",
                unique: true,
                filter: "[AppUserId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ChargeTags_AppUserId",
                table: "ChargeTags");

            migrationBuilder.DropColumn(
                name: "RegisteredTime",
                table: "AspNetUsers");

            migrationBuilder.CreateIndex(
                name: "IX_ChargeTags_AppUserId",
                table: "ChargeTags",
                column: "AppUserId");
        }
    }
}
