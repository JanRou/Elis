using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer {

    public interface IProgramSettings {
        string GetConnectionString(string connStr);
        string GetAppSettings(string key);
    }

    public class ProgramSettings : IProgramSettings {
        public string GetConnectionString(string connStr) {
            return ConfigurationManager.ConnectionStrings[connStr].ConnectionString;
        }
        public string GetAppSettings(string key) {
            return ConfigurationManager.AppSettings[key] ?? string.Empty;
        }
    }
}
