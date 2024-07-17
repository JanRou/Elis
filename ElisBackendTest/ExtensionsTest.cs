using ElisBackend.Core.Domain.Entities;
using ElisBackend.Core.Domain.Entities.Filters;
using ElisBackend.Extensions;
using Npgsql;

namespace ElisBackendTest
{
    public class ExtensionsTest {

        [Fact]
        //[Theory]
        public async Task QueryParametersFromClassTest() {
            // Arrange
            var dut = new FilterStock { Name = "Novo%", Skip = 10 };

            // Act
            var result = new List<NpgsqlParameter>().QueryParametersFromClass<FilterStock>(dut);

            // Assert
            Assert.NotNull(result);
            Assert.Contains(result, p => string.Compare(p.ParameterName, "namein") == 0 && p.Value == "Novo%");
            Assert.Contains(result, p => p.ParameterName == "isinin" && p.Value == "");
            Assert.Contains(result, p => p.ParameterName == "currencycodein" && p.Value == "");
            Assert.Contains(result, p => p.ParameterName == "exchangenamein" && p.Value == "");
            Assert.Contains(result, p => p.ParameterName == "orderbyin" && p.Value == "");
            Assert.Contains(result, p => p.ParameterName == "takein" && (int)p.Value == 0);
            Assert.Contains(result, p => p.ParameterName == "skipin" && (int)p.Value == 10);
        }

        [Fact]
        //[Theory]
        public async Task CreateParameterNamesDefaultTest() {
            // Arrange
            var filter = new FilterStock { Name = "Novo%", Skip = 10 };
            var dut = new List<NpgsqlParameter>().QueryParametersFromClass<FilterStock>(filter);

            // Act
            var result = dut.CreateParameterNames();

            // Assert
            Assert.NotEmpty(result);
            Assert.Contains("@namein", result);
            Assert.Contains("@isinin", result);
            Assert.Contains("@currencycodein", result);
            Assert.Contains("@exchangenamein", result);
            Assert.Contains("@skipin", result);
            Assert.Contains("@takein", result);
        }

        [Fact]
        //[Theory]
        public void CreateParameterNamesPreEndTest() {
            // Arrange
            var filter = new FilterStock { Name = "Novo%", Skip = 10 };
            var dut = new List<NpgsqlParameter>()
                .QueryParametersFromClass<FilterStock>(filter, prepend: "pre_", ending: "_end");

            // Act
            var result = dut.CreateParameterNames();

            // Assert
            Assert.NotEmpty(result);
            Assert.Contains("@pre_name_end", result);
            Assert.Contains("@pre_isin_end", result);
            Assert.Contains("@pre_currencycode_end", result);
            Assert.Contains("@pre_exchangename_end", result);
            Assert.Contains("@pre_skip_end", result);
            Assert.Contains("@pre_take_end", result);
        }

        public class TestFilter {
            public int I { get; set; }
            public int? In { get; set; } = null;
            public uint Ui { get; set; }
            public uint? Uin { get; set; } = null;
            public decimal Dec { get; set; }
            public decimal? Decn { get; set; } = null;
            public double Dou { get; set; }
            public double? Doun { get; set; } = null;
            public float Flo { get; set; }
            public float? Flon { get; set; } = null;
            public string St { get; set; } = string.Empty;
            public string? Stn { get; set; } = null;
        }

        [Fact]
        //[Theory]
        public void NullValueTest() {

            // Arrange
            var filter = new TestFilter { 
                  I= 10
                , Ui = 20
                , Dec = 0.1m
                , Dou = 0.01f
                , Flo = 0.001f
                , St = "Not empty"
            };

            // Act
            var result = new List<NpgsqlParameter>().QueryParametersFromClass<TestFilter>(filter);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(12, result.Count);
            Assert.Contains(result, p => string.Compare(p.ParameterName, "iin") == 0 
                && p.Value is int && (int) p.Value == filter.I);
            Assert.Contains(result, p => string.Compare(p.ParameterName, "inin") == 0
                && p.Value is int? && ((int?)p.Value).HasValue && (int?)p.Value == 0);
            Assert.Contains(result, p => string.Compare(p.ParameterName, "uiin") == 0 
                && p.Value is uint && (uint)p.Value == filter.Ui);
            Assert.Contains(result, p => string.Compare(p.ParameterName, "uinin") == 0
                && p.Value is uint && (uint)p.Value == 0);
            Assert.Contains(result, p => string.Compare(p.ParameterName, "decin") == 0
                && p.Value is decimal && (decimal)p.Value == filter.Dec);
            Assert.Contains(result, p => string.Compare(p.ParameterName, "decnin") == 0
                && p.Value is decimal && (decimal)p.Value == 0.0m);
            Assert.Contains(result, p => string.Compare(p.ParameterName, "douin") == 0
                && p.Value is double && (double)p.Value == filter.Dou);
            Assert.Contains(result, p => string.Compare(p.ParameterName, "dounin") == 0
                && p.Value is double && (double)p.Value == 0.0f);
            Assert.Contains(result, p => string.Compare(p.ParameterName, "floin") == 0
                && p.Value is float && (float)p.Value == filter.Flo);
            Assert.Contains(result, p => string.Compare(p.ParameterName, "flonin") == 0
                && p.Value is float && (float)p.Value == 0.0f);
            Assert.Contains(result, p => string.Compare(p.ParameterName, "stin") == 0
                && p.Value is string && string.Compare((string)p.Value, filter.St) == 0);
            Assert.Contains(result, p => string.Compare(p.ParameterName, "stnin") == 0
                && string.IsNullOrEmpty((string)p.Value) && string.Compare((string)p.Value, "") == 0 );

        }


        //[Fact]
        ////[Theory]
        //public void Test() {
        //    // Arrange

        //    // Act

        //    // Assert

        //}

    }
}