using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System.Collections.Generic;

// Requires reference to WebDriver.Support.dll
//using OpenQA.Selenium.Support.UI;

namespace NasdaqOmxScraperPoc {

    public class SeleniumPoc {

        private readonly string _url;

        public SeleniumPoc(string url) {
            _url = url;
        }

        public void GetNasdaqOmxStocklist() {
            using (IWebDriver driver = new FirefoxDriver()) {
                driver.Navigate().GoToUrl(_url);

                // Find searchSharesFormId
                IWebElement searchSharesForm = driver.FindElement(By.Id("searchSharesFormId"));

                IWebElement nordicSharesLabel = null;
                IWebElement firstNorthSharesLabel = null;
                ICollection<IWebElement> labels = searchSharesForm.FindElements(By.TagName("label"));
                foreach (IWebElement label in labels) {
                    if (label.GetAttribute("for") == "nordicShares") {
                        nordicSharesLabel = label;
                    }
                    if (label.GetAttribute("for") == "firstNorth") {
                        firstNorthSharesLabel = label;
                    }
                }
                // <label for="nordicShares" class="ui-button ui-widget ui-state-default ui-button-text-only ui-corner-left ui-state-active" role="button" aria-disabled="false" aria-pressed="true">
                //    <span class="ui-button-text">Main Market</span>
                // </label>

                // Get the resulting table of stocks id information
                // <table id="searchSharesListTable"  ... >
                //IWebElement searchSharesListTable = driver.FindElement(By.Id("searchSharesListTable"));
                //searchSharesListTable.
                if (nordicSharesLabel!= null) {
                    // Write it to a file
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Temp\SearchSharesListTable.txt")) {
                        //file.Write(searchSharesListTable.Text);
                        file.Write(nordicSharesLabel.Text);
                    }
                }

            }
        }
    }
}
