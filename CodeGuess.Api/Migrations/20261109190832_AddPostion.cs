using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodeGuess.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddPostion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Position",
                table: "Questions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Position",
                table: "Questions");
        }
    }
}
