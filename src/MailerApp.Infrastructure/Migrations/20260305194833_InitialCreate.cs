using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MailerApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EntityType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    EntityId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Action = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    UserOrSystem = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    PayloadJson = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContactLists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactLists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Contacts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Company = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CustomFieldsJson = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contacts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SenderAccounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Provider = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    EncryptedRefreshToken = table.Column<string>(type: "TEXT", nullable: true),
                    EncryptedAppPassword = table.Column<string>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SenderAccounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SendEvents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SendJobId = table.Column<int>(type: "INTEGER", nullable: true),
                    CampaignRecipientId = table.Column<int>(type: "INTEGER", nullable: true),
                    EventType = table.Column<int>(type: "INTEGER", nullable: false),
                    PayloadJson = table.Column<string>(type: "TEXT", nullable: true),
                    OccurredAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SendEvents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SuppressionList",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    Reason = table.Column<int>(type: "INTEGER", nullable: false),
                    Source = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SuppressionList", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Templates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Subject = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    BodyHtml = table.Column<string>(type: "TEXT", nullable: true),
                    BodyText = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Templates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContactListMembers",
                columns: table => new
                {
                    ContactId = table.Column<int>(type: "INTEGER", nullable: false),
                    ListId = table.Column<int>(type: "INTEGER", nullable: false),
                    JoinedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactListMembers", x => new { x.ContactId, x.ListId });
                    table.ForeignKey(
                        name: "FK_ContactListMembers_ContactLists_ListId",
                        column: x => x.ListId,
                        principalTable: "ContactLists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContactListMembers_Contacts_ContactId",
                        column: x => x.ContactId,
                        principalTable: "Contacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Campaigns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    TemplateId = table.Column<int>(type: "INTEGER", nullable: false),
                    SenderAccountId = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    DailyCap = table.Column<int>(type: "INTEGER", nullable: false),
                    DelayMsMin = table.Column<int>(type: "INTEGER", nullable: false),
                    DelayMsMax = table.Column<int>(type: "INTEGER", nullable: false),
                    ScheduledAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    StartedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    EndedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Campaigns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Campaigns_SenderAccounts_SenderAccountId",
                        column: x => x.SenderAccountId,
                        principalTable: "SenderAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Campaigns_Templates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "Templates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CampaignRecipients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CampaignId = table.Column<int>(type: "INTEGER", nullable: false),
                    ContactId = table.Column<int>(type: "INTEGER", nullable: false),
                    ListId = table.Column<int>(type: "INTEGER", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    SentAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ErrorCode = table.Column<string>(type: "TEXT", nullable: true),
                    RetryCount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignRecipients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CampaignRecipients_Campaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "Campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CampaignRecipients_Contacts_ContactId",
                        column: x => x.ContactId,
                        principalTable: "Contacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SendJobs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CampaignId = table.Column<int>(type: "INTEGER", nullable: false),
                    CampaignRecipientId = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    QueuedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Attempts = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SendJobs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SendJobs_CampaignRecipients_CampaignRecipientId",
                        column: x => x.CampaignRecipientId,
                        principalTable: "CampaignRecipients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SendJobs_Campaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "Campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CampaignRecipients_CampaignId",
                table: "CampaignRecipients",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignRecipients_ContactId",
                table: "CampaignRecipients",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_Campaigns_SenderAccountId",
                table: "Campaigns",
                column: "SenderAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Campaigns_TemplateId",
                table: "Campaigns",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_ContactListMembers_ListId",
                table: "ContactListMembers",
                column: "ListId");

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_Email",
                table: "Contacts",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SendJobs_CampaignId",
                table: "SendJobs",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_SendJobs_CampaignRecipientId",
                table: "SendJobs",
                column: "CampaignRecipientId");

            migrationBuilder.CreateIndex(
                name: "IX_SuppressionList_Email",
                table: "SuppressionList",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "ContactListMembers");

            migrationBuilder.DropTable(
                name: "SendEvents");

            migrationBuilder.DropTable(
                name: "SendJobs");

            migrationBuilder.DropTable(
                name: "SuppressionList");

            migrationBuilder.DropTable(
                name: "ContactLists");

            migrationBuilder.DropTable(
                name: "CampaignRecipients");

            migrationBuilder.DropTable(
                name: "Campaigns");

            migrationBuilder.DropTable(
                name: "Contacts");

            migrationBuilder.DropTable(
                name: "SenderAccounts");

            migrationBuilder.DropTable(
                name: "Templates");
        }
    }
}
