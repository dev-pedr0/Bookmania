using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bookmania.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initializer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Produzido",
                table: "Livros",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Produzido",
                table: "Livros");
        }
    }
}
