using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
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

        public static async void SetUp(string targetAccount, string folderSavePath, bool headless, bool firefoxProfile)
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


            string savePath;
            var folderSavePathSections = folderSavePath.Split("/");
            var maxIndex = folderSavePathSections.Length - 1;
            if (folderSavePathSections[maxIndex].Contains(targetAccount) ||
                folderSavePathSections[maxIndex].Equals(targetAccount, StringComparison.InvariantCultureIgnoreCase))
            {
                savePath = folderSavePath + "/";
            }
            else
            {
                savePath = folderSavePath + "/" + targetAccount + "/";
            }
            
            var buffer = new BufferBlock<KeyValuePair<string, string>>();
            var consumer = DownloadManager.ConsumeAsync(savePath, buffer);
            
            RunScraper(targetAccount, buffer);

            await consumer;
            
            Console.WriteLine("Processed {0} files.", consumer.Result);
            _driver.Quit();
        }

        private static void RunScraper(string targetAccount, ITargetBlock<KeyValuePair<string, string>> target)
        {
            var profilePage = new ProfilePage(_driver);
            var watch = System.Diagnostics.Stopwatch.StartNew();
            
            profilePage.GoToProfile(targetAccount);
            profilePage.GetProfilePicture(target);
            
            // profilePage.EnterStory();
            var postPage = profilePage.EnterPosts(target);
            watch.Stop();
            var enterPostTime = watch.ElapsedMilliseconds;
            Console.WriteLine("Time to enter post: " + enterPostTime/1000.00 + " seconds");
            
            watch.Restart();
            postPage.GetPostData();
            watch.Stop();
            var getPostPicturesTime = watch.ElapsedMilliseconds;
            Console.WriteLine("Time to get all post pictures: " + getPostPicturesTime/1000.00 + " seconds");
            
            Console.WriteLine("Total Program Time: " + (enterPostTime + getPostPicturesTime)/1000.00 + " seconds");
        }
    }
}