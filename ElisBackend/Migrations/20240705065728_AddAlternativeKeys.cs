using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElisBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddAlternativeKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExchangeUrl",
                table: "Exchanges",
                newName: "Url");

            migrationBuilder.RenameColumn(
                name: "Short",
                table: "Currencies",
                newName: "Code");

            migrationBuilder.AddUniqueConstraint(
                name: "AlternateKey_Name",
                table: "Exchanges",
                column: "Name");

            migrationBuilder.AddUniqueConstraint(
                name: "AlternateKey_Code",
                table: "Currencies",
                column: "Code");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AlternateKey_Name",
                table: "Exchanges");

            migrationBuilder.DropUniqueConstraint(
                name: "AlternateKey_Code",
                table: "Currencies");

            migrationBuilder.RenameColumn(
                name: "Url",
                table: "Exchanges",
                newName: "ExchangeUrl");

            migrationBuilder.RenameColumn(
                name: "Code",
                table: "Currencies",
                newName: "Short");
        }
    }
}
