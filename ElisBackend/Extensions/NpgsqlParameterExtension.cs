using ElisBackend.Application.UseCases;
using Npgsql;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace ElisBackend.Extensions {

    public static class NpgsqlParameterExtension {

        /// <summary>
        /// Creates a list of SQL parameters for EF Core SqlQueryRaw call of SQL stored procedure or function.
        /// The names are lower letters and by default not prepended anything and ending with "in".
        /// Set prepend and ending according to the named arguments in the stored procedure og function.
        /// A parameter's value is always set, so string is empty string "".
        /// Take and skip is set to 0, when not present in the filter. The SQL search procedure or function
        /// has to take all for Take set to 0
        /// </summary>
        /// <param name="filter">The filter with properties to use as parameters.</param>
        /// <param name="prepend">Text string to prepend each parameter name.</param>
        /// <param name="ending">Text string to end each parameter name.</param>
        /// <returns>The list of SQL parameters representing the filter.</returns>
        public static List<NpgsqlParameter> QueryParametersFromClass<T>(
            this List<NpgsqlParameter> parms, T t, string prepend = "", string ending = "in") where T : new() {

            if (t == null) {
                throw new ArgumentNullException(nameof(t));
            }

            Dictionary<Type, Func<object, object>> setNullValue = new Dictionary<Type, Func<object, object>>()
            {
                  { typeof(int), i => i }
                , { typeof(int?), i => ((int ?) i) ?? 0 }
                , { typeof(uint), u => u }
                , { typeof(uint?), u => ((uint?) u) ?? 0}
                , { typeof(decimal), d =>  d }
                , { typeof(decimal?), d => d ??  0.0m }
                , { typeof(double), d => d }
                , { typeof(double?), d => ((double?) d) ??  0.0f } // Note 
                , { typeof(float), f => f }
                , { typeof(float?), f => ((float?) f) ??  0.0f } // Note
                , { typeof(string), s => !string.IsNullOrEmpty((string)s) ? s : "" }
            };

            var props = t.GetType().GetProperties();            

            foreach (PropertyInfo p in props) {

                if (setNullValue.ContainsKey(p.PropertyType)) {
                    parms.Add(new NpgsqlParameter(
                            prepend + p.Name.ToLower() + ending
                        , setNullValue[p.PropertyType](p.GetValue(t)))
                    );
                }
                else {
                    throw new ArgumentException();
                }
            }

            return parms;
        }

        public static string CreateParameterNames(this List<NpgsqlParameter> parms) {
            var sb = new StringBuilder();
            bool first = true;
            foreach (NpgsqlParameter param in parms) {
                if (first) {
                    sb.Append("@");
                    first = false;
                }
                else {
                    sb.Append(",@");
                }
                sb.Append(param.ParameterName);
            }
            return sb.ToString();
        }

    }
}
