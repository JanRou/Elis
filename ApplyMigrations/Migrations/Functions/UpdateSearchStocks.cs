using FluentMigrator;

namespace ApplyMigrations.Migrations.Procedures {
    [Migration(20240705090700, "UpdateSearchStocks.plpgsql")]
    public class UpdateSearchStocks : Migration {
        public override void Up() {
            Execute.EmbeddedScript("UpdateSearchStocks.plpgsql");
        }
        public override void Down() {
            // Bruges aldrig
            throw new NotImplementedException();
        }
    }
}