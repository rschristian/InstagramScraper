using System;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium;

namespace Selenium
{
    public class Scraper
    {
        public static void WebScraper()
        {
            var options = new FirefoxOptions();
//            options.AddArgument("--headless");
            IWebDriver driver = new FirefoxDriver(options);

            driver.Navigate().GoToUrl("https://www.instagram.com/gwenddalyn/");
            var query = driver.FindElement(By.CssSelector("div._bz0w a")).GetAttribute("href");
            Console.WriteLine("Post link: " + query);
            driver.FindElement(By.CssSelector("div._bz0w")).Click();
            System.Threading.Thread.Sleep(5000);
            Console.WriteLine(driver.Url);
            driver.Quit(); 
        }
    }
}