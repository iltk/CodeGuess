using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodeGuess.Api.Migrations
{
    /// <inheritdoc />
    public partial class ChangeFinishedStateToTwo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // CorrectAnswer (2) was removed, so Finished moves from 3 to 2
            migrationBuilder.Sql("UPDATE GameSessions SET State = 2 WHERE State = 3;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE GameSessions SET State = 3 WHERE State = 2;");
        }
    }
}
