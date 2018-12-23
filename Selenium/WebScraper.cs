using System;
using System.IO;
using System.Net;
using Gtk;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
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
            var optionsFireFox = new FirefoxOptions();
            var optionsChrome = new ChromeOptions();
            if (headless) { optionsFireFox.AddArgument("--headless"); optionsChrome.AddArgument("headless");}
            
//            optionsFireFox.SetPreference("permissions.default.image", 2);
//            optionsFireFox.SetPreference("dom.ipc.plugins.enabled.libflashplayer.so", false);
            
//            _driver = new FirefoxDriver(options);
            
            
            optionsChrome.AddUserProfilePreference("profile.default_content_setting_values.images", 2);
            _driver = new ChromeDriver(optionsChrome);
            
            _path = _path + targetAccount + "/";

            RunScraper(targetAccount);
        }

        private static void RunScraper(string targetAccount){
            var resourcesDictionary = new UriNameDictionary();
            var profilePage = new ProfilePage(_driver);
            var watch = System.Diagnostics.Stopwatch.StartNew();
            
            profilePage.GoToProfile(targetAccount);
            profilePage.GetProfilePicture(resourcesDictionary);
//            profilePage.EnterStory();
            var postPage = profilePage.EnterPosts();
            watch.Stop();
            var enterPostTime = watch.ElapsedMilliseconds;
            Console.WriteLine("Time to enter post: " + enterPostTime/1000.00 + " seconds");
            
            watch.Restart();
            postPage.GetPostData(resourcesDictionary);
            watch.Stop();
            var getPostPicturesTime = watch.ElapsedMilliseconds;
            Console.WriteLine("Time to get all post pictures: " + getPostPicturesTime/1000.00 + " seconds");
            
            watch.Restart();
            foreach (var entry in resourcesDictionary)
            {
                if(!File.Exists(_path)) {Directory.CreateDirectory(_path);}
                if (File.Exists(_path + entry.Key + ".mp4") || File.Exists(_path + entry.Key + ".jpg")) continue;
                if (entry.Value.Contains(".mp4"))
                {
                    WebClient.DownloadFile(entry.Value, _path + entry.Key + ".mp4");
                }
                else
                {
                    WebClient.DownloadFile(entry.Value, _path + entry.Key + ".jpg");
                }
            }
            watch.Stop();
            var downloadPicturesTime = watch.ElapsedMilliseconds;
            Console.WriteLine("Time to download pictures: " + downloadPicturesTime/1000.00 + " seconds");
            Console.WriteLine("Total Program Time: " + (enterPostTime + getPostPicturesTime +
                                                        downloadPicturesTime)/1000.00 + " seconds");
            _driver.Quit();
        }
    }
}