using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificationsAndAccountSecurity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FailedLoginAttempts",
                table: "Accounts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Accounts",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LockoutUntil",
                table: "Accounts",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "NotificationsEnabled",
                table: "Accounts",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    NotificationID = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountID = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Message = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    RelatedEntityId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.NotificationID);
                    table.ForeignKey(
                        name: "FK_Notifications_Accounts_AccountID",
                        column: x => x.AccountID,
                        principalTable: "Accounts",
                        principalColumn: "AccountID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_AccountID_IsRead",
                table: "Notifications",
                columns: new[] { "AccountID", "IsRead" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropColumn(
                name: "FailedLoginAttempts",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "LockoutUntil",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "NotificationsEnabled",
                table: "Accounts");
        }
    }
}
