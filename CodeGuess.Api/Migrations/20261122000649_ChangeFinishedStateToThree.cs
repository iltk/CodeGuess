using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodeGuess.Api.Migrations
{
    /// <inheritdoc />
    public partial class ChangeFinishedStateToThree : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Leaderboard (3) was removed, so Finished moves from 4 to 3
            migrationBuilder.Sql("UPDATE GameSessions SET State = 3 WHERE State = 4;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE GameSessions SET State = 4 WHERE State = 3;");
        }
    }
}
