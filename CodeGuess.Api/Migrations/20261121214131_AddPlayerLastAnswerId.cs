using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodeGuess.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddPlayerLastAnswerId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LastAnswerId",
                table: "Players",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastAnswerId",
                table: "Players");
        }
    }
}
