using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElisBackend.Migrations
{
    /// <inheritdoc />
    public partial class UniqueExchangeNameAndCurrecyCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Exchanges_Name",
                table: "Exchanges",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Currencies_Name",
                table: "Currencies",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Exchanges_Name",
                table: "Exchanges");

            migrationBuilder.DropIndex(
                name: "IX_Currencies_Name",
                table: "Currencies");
        }
    }
}
