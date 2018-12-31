using System;
using System.Collections.Generic;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Selenium.PageObjects;
using Selenium.Utility;

namespace Selenium
{
    public static class WebScraper
    {
        private static IWebDriver _driver;
        
        private static string _path;
        

        public static void SetUp(string targetAccount, bool headless, bool firefoxProfile)
        {
            if (firefoxProfile)
            {
                var optionsFireFox = new FirefoxOptions();
                optionsFireFox.SetPreference("permissions.default.image", 2);
                optionsFireFox.SetPreference("dom.ipc.plugins.enabled.libflashplayer.so", false);
                if (headless) { optionsFireFox.AddArgument("--headless"); }
                _driver = new FirefoxDriver(optionsFireFox);
            }
            else
            {
                var optionsChrome = new ChromeOptions();
                optionsChrome.AddUserProfilePreference("profile.default_content_setting_values.images", 2);
            
                if (headless) { optionsChrome.AddArgument("headless"); }
                _driver = new ChromeDriver(optionsChrome);
            }
            
            const string userSaveLocation = "/home/ryun/Pictures/";
            
            _path = userSaveLocation + targetAccount + "/";
            RunScraper(targetAccount, _path);
        }

        private static void RunScraper(string targetAccount, string fileSavePath){
            var downloadQueue = new Queue<KeyValuePair<string, string>>();
            var resourcesDictionary = new UriNameDictionary();
            var profilePage = new ProfilePage(_driver);
            var watch = System.Diagnostics.Stopwatch.StartNew();
            
            profilePage.GoToProfile(targetAccount);
            profilePage.GetProfilePicture(downloadQueue);
            // profilePage.EnterStory();
            var postPage = profilePage.EnterPosts(fileSavePath, downloadQueue);
            watch.Stop();
            var enterPostTime = watch.ElapsedMilliseconds;
            Console.WriteLine("Time to enter post: " + enterPostTime/1000.00 + " seconds");
            
            watch.Restart();
            postPage.GetPostData(resourcesDictionary);
            watch.Stop();
            var getPostPicturesTime = watch.ElapsedMilliseconds;
            Console.WriteLine("Time to get all post pictures: " + getPostPicturesTime/1000.00 + " seconds");
            
            Console.WriteLine("Total Program Time: " + (enterPostTime + getPostPicturesTime)/1000.00 + " seconds");
            _driver.Quit();
        }
    }
}