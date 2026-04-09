using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDealsReviewsAndExtensions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applications_SkillOffers_OfferID",
                table: "Applications");

            migrationBuilder.DropIndex(
                name: "IX_Applications_ApplicantID_OfferID",
                table: "Applications");

            migrationBuilder.AlterColumn<Guid>(
                name: "OfferID",
                table: "Applications",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "SkillRequestID",
                table: "Applications",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TelegramLinkToken",
                table: "Accounts",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Deals",
                columns: table => new
                {
                    DealID = table.Column<Guid>(type: "uuid", nullable: false),
                    ApplicationID = table.Column<Guid>(type: "uuid", nullable: false),
                    InitiatorID = table.Column<Guid>(type: "uuid", nullable: false),
                    PartnerID = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Deals", x => x.DealID);
                    table.ForeignKey(
                        name: "FK_Deals_Accounts_InitiatorID",
                        column: x => x.InitiatorID,
                        principalTable: "Accounts",
                        principalColumn: "AccountID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Deals_Accounts_PartnerID",
                        column: x => x.PartnerID,
                        principalTable: "Accounts",
                        principalColumn: "AccountID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Deals_Applications_ApplicationID",
                        column: x => x.ApplicationID,
                        principalTable: "Applications",
                        principalColumn: "ApplicationID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    ReviewID = table.Column<Guid>(type: "uuid", nullable: false),
                    DealID = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorID = table.Column<Guid>(type: "uuid", nullable: false),
                    TargetID = table.Column<Guid>(type: "uuid", nullable: false),
                    Rating = table.Column<int>(type: "integer", nullable: false),
                    Comment = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.ReviewID);
                    table.ForeignKey(
                        name: "FK_Reviews_Accounts_AuthorID",
                        column: x => x.AuthorID,
                        principalTable: "Accounts",
                        principalColumn: "AccountID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reviews_Accounts_TargetID",
                        column: x => x.TargetID,
                        principalTable: "Accounts",
                        principalColumn: "AccountID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reviews_Deals_DealID",
                        column: x => x.DealID,
                        principalTable: "Deals",
                        principalColumn: "DealID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Applications_ApplicantID_OfferID",
                table: "Applications",
                columns: new[] { "ApplicantID", "OfferID" },
                unique: true,
                filter: "\"OfferID\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_ApplicantID_SkillRequestID",
                table: "Applications",
                columns: new[] { "ApplicantID", "SkillRequestID" },
                unique: true,
                filter: "\"SkillRequestID\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_SkillRequestID",
                table: "Applications",
                column: "SkillRequestID");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_TelegramLinkToken",
                table: "Accounts",
                column: "TelegramLinkToken",
                unique: true,
                filter: "\"TelegramLinkToken\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Deals_ApplicationID",
                table: "Deals",
                column: "ApplicationID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Deals_InitiatorID",
                table: "Deals",
                column: "InitiatorID");

            migrationBuilder.CreateIndex(
                name: "IX_Deals_PartnerID",
                table: "Deals",
                column: "PartnerID");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_AuthorID",
                table: "Reviews",
                column: "AuthorID");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_DealID_AuthorID",
                table: "Reviews",
                columns: new[] { "DealID", "AuthorID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_TargetID",
                table: "Reviews",
                column: "TargetID");

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_SkillOffers_OfferID",
                table: "Applications",
                column: "OfferID",
                principalTable: "SkillOffers",
                principalColumn: "OfferID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_SkillRequests_SkillRequestID",
                table: "Applications",
                column: "SkillRequestID",
                principalTable: "SkillRequests",
                principalColumn: "RequestID",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applications_SkillOffers_OfferID",
                table: "Applications");

            migrationBuilder.DropForeignKey(
                name: "FK_Applications_SkillRequests_SkillRequestID",
                table: "Applications");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "Deals");

            migrationBuilder.DropIndex(
                name: "IX_Applications_ApplicantID_OfferID",
                table: "Applications");

            migrationBuilder.DropIndex(
                name: "IX_Applications_ApplicantID_SkillRequestID",
                table: "Applications");

            migrationBuilder.DropIndex(
                name: "IX_Applications_SkillRequestID",
                table: "Applications");

            migrationBuilder.DropIndex(
                name: "IX_Accounts_TelegramLinkToken",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "SkillRequestID",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "TelegramLinkToken",
                table: "Accounts");

            migrationBuilder.AlterColumn<Guid>(
                name: "OfferID",
                table: "Applications",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Applications_ApplicantID_OfferID",
                table: "Applications",
                columns: new[] { "ApplicantID", "OfferID" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_SkillOffers_OfferID",
                table: "Applications",
                column: "OfferID",
                principalTable: "SkillOffers",
                principalColumn: "OfferID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
