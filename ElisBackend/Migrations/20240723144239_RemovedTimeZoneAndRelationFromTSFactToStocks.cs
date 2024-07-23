using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ElisBackend.Migrations
{
    /// <inheritdoc />
    public partial class RemovedTimeZoneAndRelationFromTSFactToStocks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TimeSerieFacts_Stocks_StockId",
                table: "TimeSerieFacts");

            migrationBuilder.DropTable(
                name: "TimeZones");

            migrationBuilder.DropPrimaryKey(
                name: "TimeSerieFact_PK",
                table: "TimeSerieFacts");

            migrationBuilder.DropIndex(
                name: "IX_TimeSerieFacts_StockId",
                table: "TimeSerieFacts");

            migrationBuilder.DropColumn(
                name: "StockId",
                table: "TimeSerieFacts");

            migrationBuilder.AddColumn<int>(
                name: "StockDaoId",
                table: "TimeSeries",
                type: "integer",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "TimeSerieFact_PK",
                table: "TimeSerieFacts",
                columns: new[] { "TimeSerieId", "DateId" });

            migrationBuilder.CreateIndex(
                name: "IX_TimeSeries_StockDaoId",
                table: "TimeSeries",
                column: "StockDaoId");

            migrationBuilder.CreateIndex(
                name: "IX_Stocks_Isin",
                table: "Stocks",
                column: "Isin",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TimeSeries_Stocks_StockDaoId",
                table: "TimeSeries",
                column: "StockDaoId",
                principalTable: "Stocks",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TimeSeries_Stocks_StockDaoId",
                table: "TimeSeries");

            migrationBuilder.DropIndex(
                name: "IX_TimeSeries_StockDaoId",
                table: "TimeSeries");

            migrationBuilder.DropPrimaryKey(
                name: "TimeSerieFact_PK",
                table: "TimeSerieFacts");

            migrationBuilder.DropIndex(
                name: "IX_Stocks_Isin",
                table: "Stocks");

            migrationBuilder.DropColumn(
                name: "StockDaoId",
                table: "TimeSeries");

            migrationBuilder.AddColumn<int>(
                name: "StockId",
                table: "TimeSerieFacts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "TimeSerieFact_PK",
                table: "TimeSerieFacts",
                columns: new[] { "TimeSerieId", "StockId", "DateId" });

            migrationBuilder.CreateTable(
                name: "TimeZones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ExchangeId = table.Column<int>(type: "integer", nullable: false),
                    FromUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Offset = table.Column<int>(type: "integer", nullable: false),
                    ToUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("TimeZoneId_PK", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TimeZones_Exchanges_ExchangeId",
                        column: x => x.ExchangeId,
                        principalTable: "Exchanges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TimeSerieFacts_StockId",
                table: "TimeSerieFacts",
                column: "StockId");

            migrationBuilder.CreateIndex(
                name: "IX_TimeZones_ExchangeId",
                table: "TimeZones",
                column: "ExchangeId");

            migrationBuilder.AddForeignKey(
                name: "FK_TimeSerieFacts_Stocks_StockId",
                table: "TimeSerieFacts",
                column: "StockId",
                principalTable: "Stocks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
