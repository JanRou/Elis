using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplyMigrations {

    [Migration(20180430121800)]
    public class AddLogTable : Migration {
        public override void Up() {
            //opret noget i databasen
        }

        public override void Down() {
            throw new NotImplementedException();
        }

    }
}
