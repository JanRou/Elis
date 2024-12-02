using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElisBackend.Migrations
{
    /// <inheritdoc />
    public partial class RefactoredRelationStockTimeSeries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TimeSeries_Stocks_StockDaoId",
                table: "TimeSeries");

            migrationBuilder.DropIndex(
                name: "IX_TimeSeries_StockDaoId",
                table: "TimeSeries");

            migrationBuilder.DropIndex(
                name: "IX_Currencies_Name",
                table: "Currencies");

            migrationBuilder.DropColumn(
                name: "StockDaoId",
                table: "TimeSeries");

            migrationBuilder.AddColumn<int>(
                name: "StockId",
                table: "TimeSeries",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TimeSeries_StockId",
                table: "TimeSeries",
                column: "StockId");

            migrationBuilder.CreateIndex(
                name: "IX_Currencies_Code",
                table: "Currencies",
                column: "Code",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TimeSeries_Stocks_StockId",
                table: "TimeSeries",
                column: "StockId",
                principalTable: "Stocks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TimeSeries_Stocks_StockId",
                table: "TimeSeries");

            migrationBuilder.DropIndex(
                name: "IX_TimeSeries_StockId",
                table: "TimeSeries");

            migrationBuilder.DropIndex(
                name: "IX_Currencies_Code",
                table: "Currencies");

            migrationBuilder.DropColumn(
                name: "StockId",
                table: "TimeSeries");

            migrationBuilder.AddColumn<int>(
                name: "StockDaoId",
                table: "TimeSeries",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TimeSeries_StockDaoId",
                table: "TimeSeries",
                column: "StockDaoId");

            migrationBuilder.CreateIndex(
                name: "IX_Currencies_Name",
                table: "Currencies",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TimeSeries_Stocks_StockDaoId",
                table: "TimeSeries",
                column: "StockDaoId",
                principalTable: "Stocks",
                principalColumn: "Id");
        }
    }
}
