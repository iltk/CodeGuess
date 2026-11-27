using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodeGuess.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddPlayerLastRoundPoints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "LastRoundPoints",
                table: "Players",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastRoundPoints",
                table: "Players");
        }
    }
}
