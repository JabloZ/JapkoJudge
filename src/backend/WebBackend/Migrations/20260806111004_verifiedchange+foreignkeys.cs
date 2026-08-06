using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebBackend.Migrations
{
    /// <inheritdoc />
    public partial class verifiedchangeforeignkeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Verified",
                table: "Challenges",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Submissions_ManifestId",
                table: "Submissions",
                column: "ManifestId");

            migrationBuilder.CreateIndex(
                name: "IX_Submissions_OwnerId",
                table: "Submissions",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Submissions_ChallengesLanguages_ManifestId",
                table: "Submissions",
                column: "ManifestId",
                principalTable: "ChallengesLanguages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Submissions_Users_OwnerId",
                table: "Submissions",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Submissions_ChallengesLanguages_ManifestId",
                table: "Submissions");

            migrationBuilder.DropForeignKey(
                name: "FK_Submissions_Users_OwnerId",
                table: "Submissions");

            migrationBuilder.DropIndex(
                name: "IX_Submissions_ManifestId",
                table: "Submissions");

            migrationBuilder.DropIndex(
                name: "IX_Submissions_OwnerId",
                table: "Submissions");

            migrationBuilder.DropColumn(
                name: "Verified",
                table: "Challenges");
        }
    }
}
