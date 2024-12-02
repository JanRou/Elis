using FluentMigrator;

namespace ApplyMigrations.Migrations.Procedures {
    [Migration(20240224142500, "AddSkipAndTakeSearchStocks.plpgsql")]
    public class AddSkipAndTakeSearchStocks : Migration {
        public override void Up() {
            Execute.EmbeddedScript("AddSkipAndTakeSearchStocks.plpgsql");
        }
        public override void Down() {
            // Never used
            throw new NotImplementedException();
        }
    }

}