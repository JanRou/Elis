using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElisBackend.Migrations
{
    /// <inheritdoc />
    public partial class TimeSerieFactsCombinedKeyRemoved : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Dates_DateTimeUtc",
                table: "Dates",
                column: "DateTimeUtc",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Dates_DateTimeUtc",
                table: "Dates");
        }
    }
}
