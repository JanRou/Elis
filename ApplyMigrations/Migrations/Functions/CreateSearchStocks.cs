using FluentMigrator;

namespace ApplyMigrations.Migrations.Procedures {
    [Migration(20240223181800, "CreateSearchStocks.plpgsql")]
    public class CreateSearchStocks : Migration {
        public override void Up() {
            Execute.EmbeddedScript("CreateSearchStocks.plpgsql");
        }
        public override void Down() {
            // Bruges aldrig
            throw new NotImplementedException();
        }
    }
}