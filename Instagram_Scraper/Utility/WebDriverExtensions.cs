using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Instagram_Scraper.Utility
{
    public class WebDriverExtensions
    {
        private readonly IWebDriver _driver;

        public WebDriverExtensions(IWebDriver driver)
        {
            _driver = driver;
        }

        public IWebElement WaitForElement(By by, int timeoutInMilliSeconds)
        {
            try
            {
                if (timeoutInMilliSeconds <= 0) _driver.FindElement(by);
                var wait = new WebDriverWait(_driver, TimeSpan.FromMilliseconds(timeoutInMilliSeconds));
                return wait.Until(drv => drv.FindElement(by));
            }
            catch (WebDriverTimeoutException)
            {
                return null;
            }
        }

        public IWebElement SafeFindElement(string selector)
        {
            try
            {
                return _driver.FindElement(By.CssSelector(selector));
            }
            catch (NoSuchElementException)
            {
                return null;
            }
        }

        public IEnumerable<IWebElement> SafeFindElements(string selector)
        {
            return _driver.FindElements(By.CssSelector(selector));
        }

        public string RefineTimeStamp()
        {
            var timeStamp = SafeFindElement("time[datetime]").GetAttribute("datetime");
            timeStamp = timeStamp.Substring(0, 10) + " " + timeStamp.Substring(11, 8);
            return timeStamp;
        }

        public string RefineDateStamp()
        {
            var timeStamp = SafeFindElement("time[datetime]").GetAttribute("datetime");
            timeStamp = timeStamp.Substring(0, 10);
            return timeStamp;
        }
    }
}