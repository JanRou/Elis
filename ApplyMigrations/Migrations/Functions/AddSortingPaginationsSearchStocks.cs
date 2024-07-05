using FluentMigrator;

namespace ApplyMigrations.Migrations.Procedures {
    [Migration(20240705134500, "AddSortingPaginationsSearchStocks.plpgsql")]
    public class AddSortingPaginationsSearchStocks : Migration {
        public override void Up() {
            Execute.EmbeddedScript("AddSortingPaginationsSearchStocks.plpgsql");
        }
        public override void Down() {
            // Bruges aldrig
            throw new NotImplementedException();
        }
    }
}