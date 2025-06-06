using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bookmania.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenomearListaEspera : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "ListasDeEspera",
                newName: "ListaEspera");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "ListaEspera",
                newName: "ListasDeEspera");
        }
    }
}
