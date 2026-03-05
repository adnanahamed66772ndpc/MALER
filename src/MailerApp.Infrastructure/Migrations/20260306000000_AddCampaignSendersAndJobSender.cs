using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MailerApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCampaignSendersAndJobSender : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SenderAccountId",
                table: "SendJobs",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CampaignSenders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CampaignId = table.Column<int>(type: "INTEGER", nullable: false),
                    SenderAccountId = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxEmails = table.Column<int>(type: "INTEGER", nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignSenders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CampaignSenders_Campaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "Campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CampaignSenders_SenderAccounts_SenderAccountId",
                        column: x => x.SenderAccountId,
                        principalTable: "SenderAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CampaignSenders_CampaignId",
                table: "CampaignSenders",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignSenders_SenderAccountId",
                table: "CampaignSenders",
                column: "SenderAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_SendJobs_SenderAccountId",
                table: "SendJobs",
                column: "SenderAccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_SendJobs_SenderAccounts_SenderAccountId",
                table: "SendJobs",
                column: "SenderAccountId",
                principalTable: "SenderAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SendJobs_SenderAccounts_SenderAccountId",
                table: "SendJobs");

            migrationBuilder.DropTable(
                name: "CampaignSenders");

            migrationBuilder.DropIndex(
                name: "IX_SendJobs_SenderAccountId",
                table: "SendJobs");

            migrationBuilder.DropColumn(
                name: "SenderAccountId",
                table: "SendJobs");
        }
    }
}
