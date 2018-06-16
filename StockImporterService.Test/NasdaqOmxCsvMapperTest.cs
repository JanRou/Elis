using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StockImportService.Pipeline;
using StockImportService.Importers.NasdaqOmxCsv;
using System.Collections.Generic;

namespace StockImporterService.Test {
    using PipelineDto = PipelineDto<string, NasdaqOmxCsvlineDto>;

    [TestClass]
    public class NasdaqOmxCsvMapperTest {

        [TestMethod]
        public void GetDecimalTest() {
            // Arrange
            NasdaqOmxCsvMapper dut = new NasdaqOmxCsvMapper(2);
            // Act
            int result1 = dut.GetDecimal("174,5");
            int result2 = dut.GetDecimal("174,50");
            // Assert
            Assert.AreEqual(17450, result1);
            Assert.AreEqual(17450, result2);
        }

        [TestMethod]
        public void MapperTest() {
            // Arrange  2010-11-02;43,0;43,25;0;0;0;43,55;0;;;0;
            NasdaqOmxCsvMapper dut = new NasdaqOmxCsvMapper(2);
            List<PipelineDto> input = new List<PipelineDto>() {
                new PipelineDto() {
                    In = "2017-02-24;174,0;175,0;180,0;180,0;172,5;174,0;175,84;48128;8451395,5;488;"
                ,    Out = new NasdaqOmxCsvlineDto()
                }
                ,new PipelineDto() {
                    In = "2010-11-02;43,0;43,25;0;0;0;43,55;0;;;0;"
                ,    Out = new NasdaqOmxCsvlineDto()
                }
            };
            // Act
            IEnumerable<PipelineDto> tempList = dut.Execute(input);
            int i = 0;
            List<PipelineDto> result = new List<PipelineDto>();
            foreach (PipelineDto p in tempList) {
                i++;
                result.Add(p);
            }
            // Assert
            Assert.AreEqual(2, result.Count); // two rows
            // Row 1 asserts
            Assert.AreEqual(new DateTime(2017, 02, 24).ToUniversalTime(), result[0].Out.DateUtc);
            Assert.AreEqual(17400, result[0].Out.ClosingPrice);
            // Row 2 asserts
            Assert.AreEqual(new DateTime(2010, 11, 02).ToUniversalTime(), result[1].Out.DateUtc);
            Assert.AreEqual(4355, result[1].Out.ClosingPrice);
        }

        //[TestMethod]
        //public void Test() {
        //    // Arrange

        //    // Act

        //    // Assert
        //}

    }
}
