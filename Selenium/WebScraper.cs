using System;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium;
using Selenium.PageObjects;

namespace Selenium
{
    public class WebScraper
    {
        private static IWebDriver _driver;

        public static void SetUp(string targetAccount, bool headless)
        {
            var options = new FirefoxOptions();
            if (headless) { options.AddArgument("--headless");}
            
            options.SetPreference("pdfjs.enabledCache.state",false);
            options.SetPreference("browser.helperApps.neverAsk.saveToDisk","png");
            
            _driver = new FirefoxDriver(options);
//            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);

            RunScraper(targetAccount);
        }

        private static void RunScraper(string targetAccount){
            var profilePage = new ProfilePage(_driver);
            profilePage.GoToProfile(targetAccount);
            profilePage.GetProfilePicture();
//            profilePage.EnterStory();
            var postPage = profilePage.EnterPosts();
            System.Threading.Thread.Sleep(150);
            postPage.GetPostData();
            
                     
            _driver.Quit();
        }
    }
}