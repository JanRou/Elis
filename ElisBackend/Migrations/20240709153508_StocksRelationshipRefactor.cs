using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElisBackend.Migrations
{
    /// <inheritdoc />
    public partial class StocksRelationshipRefactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AlternateKey_Name",
                table: "Exchanges");

            migrationBuilder.DropUniqueConstraint(
                name: "AlternateKey_Code",
                table: "Currencies");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddUniqueConstraint(
                name: "AlternateKey_Name",
                table: "Exchanges",
                column: "Name");

            migrationBuilder.AddUniqueConstraint(
                name: "AlternateKey_Code",
                table: "Currencies",
                column: "Code");
        }
    }
}
