using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    AccountID = table.Column<Guid>(type: "uuid", nullable: false),
                    Login = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    TelegramID = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    IsAdmin = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.AccountID);
                });

            migrationBuilder.CreateTable(
                name: "SkillsCatalog",
                columns: table => new
                {
                    SkillID = table.Column<Guid>(type: "uuid", nullable: false),
                    SkillName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Epithet = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkillsCatalog", x => x.SkillID);
                });

            migrationBuilder.CreateTable(
                name: "Educations",
                columns: table => new
                {
                    EducationID = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountID = table.Column<Guid>(type: "uuid", nullable: false),
                    InstitutionName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    DegreeField = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    YearCompleted = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Educations", x => x.EducationID);
                    table.ForeignKey(
                        name: "FK_Educations_Accounts_AccountID",
                        column: x => x.AccountID,
                        principalTable: "Accounts",
                        principalColumn: "AccountID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserProfiles",
                columns: table => new
                {
                    AccountID = table.Column<Guid>(type: "uuid", nullable: false),
                    FullName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PhotoURL = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ContactInfo = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    LastSeenOnline = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Description = table.Column<string>(type: "character varying(3000)", maxLength: 3000, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfiles", x => x.AccountID);
                    table.ForeignKey(
                        name: "FK_UserProfiles_Accounts_AccountID",
                        column: x => x.AccountID,
                        principalTable: "Accounts",
                        principalColumn: "AccountID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Proofs",
                columns: table => new
                {
                    ProofID = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountID = table.Column<Guid>(type: "uuid", nullable: false),
                    SkillID = table.Column<Guid>(type: "uuid", nullable: true),
                    FileURL = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    IsVerified = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proofs", x => x.ProofID);
                    table.ForeignKey(
                        name: "FK_Proofs_Accounts_AccountID",
                        column: x => x.AccountID,
                        principalTable: "Accounts",
                        principalColumn: "AccountID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Proofs_SkillsCatalog_SkillID",
                        column: x => x.SkillID,
                        principalTable: "SkillsCatalog",
                        principalColumn: "SkillID",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "SkillOffers",
                columns: table => new
                {
                    OfferID = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountID = table.Column<Guid>(type: "uuid", nullable: false),
                    SkillID = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Details = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkillOffers", x => x.OfferID);
                    table.ForeignKey(
                        name: "FK_SkillOffers_Accounts_AccountID",
                        column: x => x.AccountID,
                        principalTable: "Accounts",
                        principalColumn: "AccountID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SkillOffers_SkillsCatalog_SkillID",
                        column: x => x.SkillID,
                        principalTable: "SkillsCatalog",
                        principalColumn: "SkillID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SkillRequests",
                columns: table => new
                {
                    RequestID = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountID = table.Column<Guid>(type: "uuid", nullable: false),
                    SkillID = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Details = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkillRequests", x => x.RequestID);
                    table.ForeignKey(
                        name: "FK_SkillRequests_Accounts_AccountID",
                        column: x => x.AccountID,
                        principalTable: "Accounts",
                        principalColumn: "AccountID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SkillRequests_SkillsCatalog_SkillID",
                        column: x => x.SkillID,
                        principalTable: "SkillsCatalog",
                        principalColumn: "SkillID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserSkills",
                columns: table => new
                {
                    AccountID = table.Column<Guid>(type: "uuid", nullable: false),
                    SkillID = table.Column<Guid>(type: "uuid", nullable: false),
                    SkillLevel = table.Column<int>(type: "integer", nullable: false),
                    IsVerified = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSkills", x => new { x.AccountID, x.SkillID });
                    table.ForeignKey(
                        name: "FK_UserSkills_Accounts_AccountID",
                        column: x => x.AccountID,
                        principalTable: "Accounts",
                        principalColumn: "AccountID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserSkills_SkillsCatalog_SkillID",
                        column: x => x.SkillID,
                        principalTable: "SkillsCatalog",
                        principalColumn: "SkillID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VerificationRequests",
                columns: table => new
                {
                    RequestID = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountID = table.Column<Guid>(type: "uuid", nullable: false),
                    ProofID = table.Column<Guid>(type: "uuid", nullable: true),
                    RequestType = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VerificationRequests", x => x.RequestID);
                    table.ForeignKey(
                        name: "FK_VerificationRequests_Accounts_AccountID",
                        column: x => x.AccountID,
                        principalTable: "Accounts",
                        principalColumn: "AccountID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VerificationRequests_Proofs_ProofID",
                        column: x => x.ProofID,
                        principalTable: "Proofs",
                        principalColumn: "ProofID",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Applications",
                columns: table => new
                {
                    ApplicationID = table.Column<Guid>(type: "uuid", nullable: false),
                    ApplicantID = table.Column<Guid>(type: "uuid", nullable: false),
                    OfferID = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Message = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Applications", x => x.ApplicationID);
                    table.ForeignKey(
                        name: "FK_Applications_Accounts_ApplicantID",
                        column: x => x.ApplicantID,
                        principalTable: "Accounts",
                        principalColumn: "AccountID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Applications_SkillOffers_OfferID",
                        column: x => x.OfferID,
                        principalTable: "SkillOffers",
                        principalColumn: "OfferID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_Login",
                table: "Accounts",
                column: "Login",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Applications_ApplicantID_OfferID",
                table: "Applications",
                columns: new[] { "ApplicantID", "OfferID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Applications_OfferID",
                table: "Applications",
                column: "OfferID");

            migrationBuilder.CreateIndex(
                name: "IX_Educations_AccountID",
                table: "Educations",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "IX_Proofs_AccountID",
                table: "Proofs",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "IX_Proofs_SkillID",
                table: "Proofs",
                column: "SkillID");

            migrationBuilder.CreateIndex(
                name: "IX_SkillOffers_AccountID",
                table: "SkillOffers",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "IX_SkillOffers_SkillID",
                table: "SkillOffers",
                column: "SkillID");

            migrationBuilder.CreateIndex(
                name: "IX_SkillRequests_AccountID",
                table: "SkillRequests",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "IX_SkillRequests_SkillID",
                table: "SkillRequests",
                column: "SkillID");

            migrationBuilder.CreateIndex(
                name: "IX_SkillsCatalog_SkillName",
                table: "SkillsCatalog",
                column: "SkillName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserSkills_SkillID",
                table: "UserSkills",
                column: "SkillID");

            migrationBuilder.CreateIndex(
                name: "IX_VerificationRequests_AccountID",
                table: "VerificationRequests",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "IX_VerificationRequests_ProofID",
                table: "VerificationRequests",
                column: "ProofID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Applications");

            migrationBuilder.DropTable(
                name: "Educations");

            migrationBuilder.DropTable(
                name: "SkillRequests");

            migrationBuilder.DropTable(
                name: "UserProfiles");

            migrationBuilder.DropTable(
                name: "UserSkills");

            migrationBuilder.DropTable(
                name: "VerificationRequests");

            migrationBuilder.DropTable(
                name: "SkillOffers");

            migrationBuilder.DropTable(
                name: "Proofs");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "SkillsCatalog");
        }
    }
}
