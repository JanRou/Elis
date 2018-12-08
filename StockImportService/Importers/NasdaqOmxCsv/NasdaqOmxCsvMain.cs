using DataAccessLayer;
using DataAccessLayer.Dto;
using DataAccessLayer.Tables;
using StockImportService.Importers;
using StockImportService.Importers.NasdaqOmxCsv;
using System;
using System.Collections.Generic;
using System.IO;

namespace StockMarketService.Importers.NasdaqOmxCsv {
    public class NasdaqOmxCsvMain {
        void Main() {
            IProgramSettings programSettings = new ProgramSettings();

            string importFolder = programSettings.GetAppSettings("ImportFolder");
            string resultFolder = programSettings.GetAppSettings("ResultFolder");

            Dictionary<string, NasdaqOmxStock> nasdaqOmxStocks = CreateNasdaqOmxStocks();

            IDal dal = new Dal(programSettings.GetConnectionString("PostgresDotNet"));

            foreach (string fileName in Directory.GetFiles(importFolder)) {

                string fileTicker = GetFileTicker(fileName);

                if (nasdaqOmxStocks.ContainsKey(fileTicker)) {
                    Console.WriteLine($"Process ticker {fileTicker} and file {fileName}");
                    dal.Open();
                    Tuple<int, int> result = PrepareStockandSerie(
                          dal
                        , new NasdaqOmxCsvDto(
                              nasdaqOmxStocks[fileTicker].Ticker
                            , nasdaqOmxStocks[fileTicker].Currency
                            , nasdaqOmxStocks[fileTicker].StockName
                            , nasdaqOmxStocks[fileTicker].Market
                            , nasdaqOmxStocks[fileTicker].ValueTypeName
                            , nasdaqOmxStocks[fileTicker].VolumeValueTypeName)
                        , nasdaqOmxStocks[fileTicker].Url
                    );
                    StreamReader streamReader = new StreamReader(fileName);

                    IImporterPipeline nasdaqOmxCsvImporter = new NasdaqOmxCsvImporterPipeline(
                          streamReader
                        , 2
                        , result.Item1
                        , DateTime.MinValue //her til her til last close er datoen for den sidste lukke kurs i databasen
                        , serieIdVolume: result.Item2
                        , dataAccessLayer: dal
                    );
                    // Open writer for time series and execute
                    dal.OpenTimeserieWriter();
                    nasdaqOmxCsvImporter.Execute();
                    dal.CloseWriter();
                    dal.Close();
                }
                else {
                    Console.WriteLine($"Don't know file ticker {fileTicker} for filename {fileName}");
                }
            }
            Console.WriteLine("Tryk en tast ...");
            var ch = Console.ReadKey();
        }

        private static string GetFileTicker(string fileName) {
            // C:\Temp\Import\SIM-2007-04-12-2017-04-12.csv
            string[] folderAndFilesname = fileName.Split('\\');
            // SIM-2007-04-12-2017-04-12.csv
            string[] splits = folderAndFilesname[folderAndFilesname.Length - 1].Split('-');
            return splits[0];
        }

        private static Dictionary<string, NasdaqOmxStock> CreateNasdaqOmxStocks() {
            return new Dictionary<string, NasdaqOmxStock>() {
                  { "PAAL_B", new NasdaqOmxStock("PAAl-B", "DKK", "Per Aarsleff B", "Nasdaq OMX Copenhagen", "2 decimals"
                        , "http://www.nasdaqomxnordic.com/aktier/microsite?Instrument=CSE3272&name=Per%20Aarslef..."
                        , "whole number") }
                , { "NOVO_B", new NasdaqOmxStock("NOVO-B", "DKK", "Novo Nordisk B", "Nasdaq OMX Copenhagen", "2 decimals"
                        , "http://www.nasdaqomxnordic.com/aktier/microsite?Instrument=CSE1158&name=Novo%20Nordisk%20B"
                        , "whole number") }
                , { "AMBU_B", new NasdaqOmxStock("AMBU-B", "DKK", "Ambu B", "Nasdaq OMX Copenhagen", "2 decimals"
                        , "http://www.nasdaqomxnordic.com/aktier/microsite?Instrument=CSE3331&name=Ambu"
                        , "whole number") }
                , { "ACARIX", new NasdaqOmxStock("ACARIX", "DKK", "Acarix", "Nasdaq OMX First North Stockholm", "2 decimals"
                        , "http://www.nasdaqomxnordic.com/aktier/microsite?Instrument=SSE130710&name=Acarix"
                        , "whole number") }
                , { "SIM", new NasdaqOmxStock("SIM", "DKK", "SimCorp", "Nasdaq OMX Copenhagen", "2 decimals"
                        , "http://www.nasdaqomxnordic.com/aktier/microsite?Instrument=CSE4806&name=SimCorp"
                        , "whole number") }
            };
        }

        private static Tuple<int, int> PrepareStockandSerie(IDal dal, NasdaqOmxCsvDto nasdaqOmxCsvDto
            , string url) {
            // Call DAL layer to create new stock and get stock id or get existing
            int stockId = GetOrCreateStock(dal, nasdaqOmxCsvDto, url);
            // Create or Get stock series for closing price and volume
            int closeSerieId = GetOrCreateSerieForStock(dal, stockId, "close", nasdaqOmxCsvDto, nasdaqOmxCsvDto.ValueTypeName);
            int volumeSerieId = GetOrCreateSerieForStock(dal, stockId, "volume", nasdaqOmxCsvDto, nasdaqOmxCsvDto.VolumeValueTypeName);
            return new Tuple<int, int>(closeSerieId, volumeSerieId);
        }

        private static int GetOrCreateStock(IDal dal, NasdaqOmxCsvDto nasdaqOmxCsvDto, string url) {
            // Try to get stock
            IStock stock = dal.GetStock(nasdaqOmxCsvDto.StockName, nasdaqOmxCsvDto.Market);
            int result = stock != null ? stock.Id : 0;
            if (result == 0) {
                // no stock, create it
                result = dal.InsertStock(nasdaqOmxCsvDto.StockName, nasdaqOmxCsvDto.Ticker
                                                , nasdaqOmxCsvDto.Market);
            }
            // Create if not exist datasource
            return result;
        }
        private static int GetOrCreateSerieForStock(IDal dal, int stockId, string serieAttribute
            , NasdaqOmxCsvDto nasdaqOmxCsvDto, string valueTypeName
        ) {
            ISerieDim serie = dal.GetSerieForStock(stockId, serieAttribute);
            int result = 0;
            if (serie != null) {
                result = serie.Id;
            }
            else {
                result = dal.InsertSerie(
                    new SerieDto(
                          name: nasdaqOmxCsvDto.Ticker + " " + serieAttribute
                        , ticker: nasdaqOmxCsvDto.Ticker
                        , valueType: valueTypeName
                        , currency: nasdaqOmxCsvDto.Currency
                        , attribute: serieAttribute)
                );
                // Associate new serie with stock
                dal.AssociateStockAndSerie(stockId, result);
            }
            return result;
        }

        private static int GetStockId(IDal dal, string stockName, string market) {
            IStock stock = dal.GetStock(stockName, market);
            return stock == null ? stock.Id : 0;
        }

    }
}
