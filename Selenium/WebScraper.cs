using System;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium;
using Selenium.PageObjects;

namespace Selenium
{
    public class WebScraper
    {
        private static IWebDriver _driver;

        public static void SetUp(string targetAccount)
        {
            var options = new FirefoxOptions();
            //options.AddArgument("--headless");
            _driver = new FirefoxDriver(options);
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);

            RunScraper(targetAccount);
        }

        private static void RunScraper(string targetAccount){
            var profilePage = new ProfilePage(_driver);
            profilePage.goToProfile(targetAccount);

//            driver.Navigate().GoToUrl("https://www.instagram.com/***REMOVED***/");
//            var hrefAddress = driver.FindElement(By.CssSelector("div._bz0w a")).GetAttribute("href");
//
//            var element = driver.FindElement(By.CssSelector("div._bz0w a"));
//            var executor = (IJavaScriptExecutor) driver;
//            executor.ExecuteScript("arguments[0].click();", element);    
//            
//            Console.WriteLine(driver.Url);
//            driver.Quit();
        }
    }
}