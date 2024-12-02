using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElisBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddStockFieldInstrumentCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InstrumentCode",
                table: "Stocks",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_TimeSeries_Name_StockId",
                table: "TimeSeries",
                columns: new[] { "Name", "StockId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TimeSeries_Name_StockId",
                table: "TimeSeries");

            migrationBuilder.DropColumn(
                name: "InstrumentCode",
                table: "Stocks");
        }
    }
}
