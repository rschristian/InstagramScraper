using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks.Dataflow;
using Instagram_Scraper.PageObjects;
using Instagram_Scraper.Utility;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;

namespace Instagram_Scraper
{
    public static class WebScraper
    {
        private static IWebDriver _driver;

        public static async void SetUp(ScraperOptions scraperOptions)
        {
            if (scraperOptions.FireFoxProfile)
            {
                var optionsFireFox = new FirefoxOptions();
                optionsFireFox.SetPreference("permissions.default.image", 2);
                optionsFireFox.SetPreference("dom.ipc.plugins.enabled.libflashplayer.so", false);

                if (scraperOptions.Headless) optionsFireFox.AddArgument("--headless");
                _driver = new FirefoxDriver(optionsFireFox);
            }
            else
            {
                var optionsChrome = new ChromeOptions();
                optionsChrome.AddUserProfilePreference("profile.default_content_setting_values.images", 2);
                optionsChrome.AddArgument("--disable-popup-blocking");
                optionsChrome.AddArgument("--window-size=1920,1080");

                if (scraperOptions.Headless) optionsChrome.AddArgument("headless");
                _driver = new ChromeDriver(optionsChrome);
            }


            string savePath;
            var homePath = Environment.OSVersion.Platform == PlatformID.Unix ||
                           Environment.OSVersion.Platform == PlatformID.MacOSX
                ? Environment.GetEnvironmentVariable("HOME")
                : Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");
            if (scraperOptions.FolderSavePath.Equals(string.Empty))
            {
                savePath = homePath + "/Pictures/" + scraperOptions.TargetAccount + "/";
            }
            else
            {
                var folderSavePathSections = scraperOptions.FolderSavePath.Split("/");
                var maxIndex = folderSavePathSections.Length - 1;
                if (folderSavePathSections[maxIndex].IndexOf(scraperOptions.TargetAccount,
                        StringComparison.OrdinalIgnoreCase) >= 0)
                    savePath = scraperOptions.FolderSavePath + "/";
                else
                    savePath = scraperOptions.FolderSavePath + "/" + scraperOptions.TargetAccount + "/";
            }


            var bufferMedia = new BufferBlock<KeyValuePair<string, string>>();
            var backgroundThreadMedia =
                new Thread(() => DownloadManager.ConsumeFilesAsync(savePath, bufferMedia)) {IsBackground = true};
            backgroundThreadMedia.Start();

            var bufferText = new BufferBlock<KeyValuePair<string, List<KeyValuePair<string, string>>>>();
            if (scraperOptions.ScrapeComments)
            {
                var backgroundThreadText =
                    new Thread(() => DownloadManager.ConsumeTextAsync(savePath, bufferText)) {IsBackground = true};
                backgroundThreadText.Start();  
            }

            ExecuteScraper(scraperOptions, bufferMedia, bufferText);

            await bufferMedia.Completion;
            if (scraperOptions.ScrapeComments) await bufferText.Completion;

            _driver.Quit();
        }

        private static void ExecuteScraper(ScraperOptions scraperOptions,
            ITargetBlock<KeyValuePair<string, string>> targetMedia,
            ITargetBlock<KeyValuePair<string, List<KeyValuePair<string, string>>>> targetText)
        {

            if (scraperOptions.OnlyScrapeStory)
            {
                OnlyScrapeStory(scraperOptions, targetMedia);
                return;
            }
            
            //Login
            var watch = Stopwatch.StartNew();
            if (!scraperOptions.Username.Equals(string.Empty))
                LoginToAccount(scraperOptions);

            //Profile Page
            var profilePage = new ProfilePage(_driver, scraperOptions.TargetAccount);
            profilePage.GetProfilePicture(targetMedia);
            if (scraperOptions.ScrapeComments) profilePage.GetProfileText(targetText);

            //Story Page
            if (scraperOptions.ScrapeStory)
            { 
                var storyPage = profilePage.EnterStory(targetMedia);
                storyPage?.SaveStoryContent();
            }
            
            var postPage = scraperOptions.ScrapeComments
                ? profilePage.EnterPosts(targetMedia, targetText)
                : profilePage.EnterPosts(targetMedia);


            //PostPage
            if (!scraperOptions.ScrapeComments)
                postPage.GetPostData();
            else
                postPage.GetPostDataWithComments();

            Console.WriteLine("Total Program Time: " + (watch.ElapsedMilliseconds) / 1000.00 + " seconds");
        }

        private static void LoginToAccount(ScraperOptions scraperOptions)
        {
            new LoginPage(_driver).Login(scraperOptions.Username, scraperOptions.Password);
        }

        private static void OnlyScrapeStory(ScraperOptions scraperOptions,
            ITargetBlock<KeyValuePair<string, string>> targetMedia)
        {
            LoginToAccount(scraperOptions);
            var storyPage = new ProfilePage(_driver, scraperOptions.TargetAccount).EnterStory(targetMedia);
            storyPage?.SaveStoryContent();
            targetMedia.Complete();
        }
    }
}