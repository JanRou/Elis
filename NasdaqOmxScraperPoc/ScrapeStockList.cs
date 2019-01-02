using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System;
using System.Collections.Generic;
using System.Threading;

// Requires reference to WebDriver.Support.dll
//using OpenQA.Selenium.Support.UI;

namespace NasdaqOmxScraper {

    public class ScrapeStockList {

        private readonly string _url;

        public ScrapeStockList(string url) {
            _url = url;
        }

        public List<(string code, string name)> GetNasdaqOmxStocklist(string marketSelect, List<string> markets, List<string> segments) {
            var result = new List<(string code, string name)>();
            using (IWebDriver driver = new FirefoxDriver()) {
                driver.Navigate().GoToUrl(_url);
                SetSearchSharesForm(driver, marketSelect, markets, segments);
                if (driver.FindElement(By.Id("searchSharesListTable")) != null) {
                    result = GetShareNames(driver.FindElement(By.Id("searchSharesListTable")));
                    //if (result.Count > 0) {
                    //    using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Temp\SearchSharesListTable.txt")) {
                    //        foreach ((string name, string code) share in result) {
                    //            file.WriteLine($"{share.name}: {share.code}");
                    //        }
                    //    }
                    //}
                }
            }
            return result;
        }

        public List<(string code, string name)> GetShareNames(IWebElement sharesTable) {
            var result = new List<(string code, string name)>();
            foreach (IWebElement a in sharesTable.FindElements(By.TagName("a"))) {
                if (!string.IsNullOrEmpty(a.GetAttribute("name"))) {
                    result.Add(( code: a.GetAttribute("name"), name: a.Text));
                }
            }
            return result;
        }

        public void SetSearchSharesForm(IWebDriver driver, string marketSelect, List<string> markets, List<string> segments) {

            IWebElement searchSharesForm = driver.FindElement(By.Id("searchSharesFormId"));
            ICollection<IWebElement> labels = searchSharesForm.FindElements(By.TagName("label"));
            foreach (IWebElement label in labels) {
                if (label.GetAttribute("for") == marketSelect) {
                    label.Click();
                    Thread.Sleep(100);
                }                
                if (markets.Contains(label.GetAttribute("for"))) {
                    label.Click();
                    Thread.Sleep(100);
                }
                if (segments.Contains(label.GetAttribute("for"))) {
                    label.Click();
                    Thread.Sleep(100);
                }
            }
            // Let the page get data ... not the best way to handle this
            Thread.Sleep(500);
        }
    }
}
