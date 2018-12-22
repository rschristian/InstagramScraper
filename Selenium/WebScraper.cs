using System;
using Gtk;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium;
using Pango;
using Selenium.PageObjects;
using Selenium.Utility;

namespace Selenium
{
    public static class WebScraper
    {
        private static IWebDriver _driver;

        public static void SetUp(string targetAccount, bool headless)
        {
            var options = new FirefoxOptions();
            if (headless) { options.AddArgument("--headless");}
            
            options.SetPreference("permissions.default.image", 2);
            options.SetPreference("dom.ipc.plugins.enabled.libflashplayer.so", false);
            
            _driver = new FirefoxDriver(options);

            RunScraper(targetAccount);
        }

        private static void RunScraper(string targetAccount){
            var resourcesDictionary = new UriNameDictionary();
            var profilePage = new ProfilePage(_driver);
            profilePage.GoToProfile(targetAccount);
            profilePage.GetProfilePicture(resourcesDictionary);
//            profilePage.EnterStory();
            var postPage = profilePage.EnterPosts();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            postPage.GetPostData(resourcesDictionary);
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("Time to get all post pictures: " + elapsedMs/1000.00 + " seconds");
            foreach (var item in resourcesDictionary)
            {
                Console.WriteLine("Item Key: " + item.Key + " Item Value: " + item.Value);
            }
            
                     
            _driver.Quit();
        }
    }
}