using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebBackend.Migrations
{
    /// <inheritdoc />
    public partial class uniques : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ChallengesLanguages_ChallengeId",
                table: "ChallengesLanguages");

            migrationBuilder.DropIndex(
                name: "IX_Challenges_OwnerId",
                table: "Challenges");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_Extension",
                table: "Languages",
                column: "Extension",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Languages_Name",
                table: "Languages",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChallengesLanguages_ChallengeId_LanguageId",
                table: "ChallengesLanguages",
                columns: new[] { "ChallengeId", "LanguageId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Challenges_OwnerId_Title",
                table: "Challenges",
                columns: new[] { "OwnerId", "Title" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Languages_Extension",
                table: "Languages");

            migrationBuilder.DropIndex(
                name: "IX_Languages_Name",
                table: "Languages");

            migrationBuilder.DropIndex(
                name: "IX_ChallengesLanguages_ChallengeId_LanguageId",
                table: "ChallengesLanguages");

            migrationBuilder.DropIndex(
                name: "IX_Challenges_OwnerId_Title",
                table: "Challenges");

            migrationBuilder.CreateIndex(
                name: "IX_ChallengesLanguages_ChallengeId",
                table: "ChallengesLanguages",
                column: "ChallengeId");

            migrationBuilder.CreateIndex(
                name: "IX_Challenges_OwnerId",
                table: "Challenges",
                column: "OwnerId");
        }
    }
}
