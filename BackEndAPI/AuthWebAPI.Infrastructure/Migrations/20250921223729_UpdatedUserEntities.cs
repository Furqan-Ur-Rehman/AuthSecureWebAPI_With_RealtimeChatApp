using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthWebAPI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedUserEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RefreshTokenExpiry",
                table: "Users",
                newName: "HashTokenExpiry");

            migrationBuilder.RenameColumn(
                name: "RefreshToken",
                table: "Users",
                newName: "HashToken");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByIp",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedHashTokenAt",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ReplacedByTokenHash",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Revoked",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "RevokedAtUtc",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevokedByIp",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedByIp",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CreatedHashTokenAt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ReplacedByTokenHash",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Revoked",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RevokedAtUtc",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RevokedByIp",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "HashTokenExpiry",
                table: "Users",
                newName: "RefreshTokenExpiry");

            migrationBuilder.RenameColumn(
                name: "HashToken",
                table: "Users",
                newName: "RefreshToken");
        }
    }
}
