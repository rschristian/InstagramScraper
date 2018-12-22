using System;
using System.IO;
using System.Net;
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
        
        private static string _path = "/home/ryun/Pictures/";
        
        private static readonly WebClient WebClient = new WebClient();

        public static void SetUp(string targetAccount, bool headless)
        {
            var options = new FirefoxOptions();
            if (headless) { options.AddArgument("--headless");}
            
            options.SetPreference("permissions.default.image", 2);
            options.SetPreference("dom.ipc.plugins.enabled.libflashplayer.so", false);
            
            _driver = new FirefoxDriver(options);
            
            _path = _path + targetAccount + "/";

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
            
            foreach (var entry in resourcesDictionary)
            {
                Directory.CreateDirectory(_path);
                if (entry.Value.Contains(".mp4"))
                {
                    WebClient.DownloadFile(entry.Value, _path + entry.Key + ".mp4");
                }
                else
                {
                    WebClient.DownloadFile(entry.Value, _path + entry.Key + ".png");
                }
               
            }
            
                     
            _driver.Quit();
        }
    }
}