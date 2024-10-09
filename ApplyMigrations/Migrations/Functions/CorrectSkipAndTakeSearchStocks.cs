using FluentMigrator;

namespace ApplyMigrations.Migrations.Procedures {
    [Migration(20240224143800, "CorrectSkipAndTakeSearchStocks.plpgsql")]
    public class CorrectSkipAndTakeSearchStocks : Migration {
        public override void Up() {
            Execute.EmbeddedScript("CorrectSkipAndTakeSearchStocks.plpgsql");
        }
        public override void Down() {
            // Never used
            throw new NotImplementedException();
        }
    }

}