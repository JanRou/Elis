using FluentMigrator;

namespace ApplyMigrations.Migrations.Procedures {
    [Migration(20241009141800, "PopulateTablesWithData.plpgsql")]
    public class PopulateTablesWithData : Migration {
        public override void Up() {
            Execute.EmbeddedScript("PopulateTablesWithData.plpgsql");
        }
        public override void Down() {
            // Never used
            throw new NotImplementedException();
        }
    }
}