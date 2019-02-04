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
                optionsChrome.AddArguments("--disable-popup-blocking", "--window-size=1920,1080", "--mute-audio");

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


            var bufferMedia = WebDriverExtensions.StartMediaService(savePath);
            if (!scraperOptions.ScrapeComments)
            {
                ExecuteScraper(scraperOptions, bufferMedia);
                await bufferMedia.Completion;
            }
            else
            {
                var bufferText = WebDriverExtensions.StartTextService(savePath);
                ExecuteScraper(scraperOptions, bufferMedia, bufferText);
                await bufferMedia.Completion;
                await bufferText.Completion;
            }

            _driver.Quit();
        }
        
        private static void ExecuteScraper(ScraperOptions scraperOptions,
            ITargetBlock<KeyValuePair<string, string>> targetMedia)
        {
            var watch = Stopwatch.StartNew();
            if (scraperOptions.OnlyScrapeStory)
            {
                OnlyScrapeStory(scraperOptions, targetMedia);
                return;
            }
            
            //Login
            if (!scraperOptions.Username.Equals(string.Empty))
                LoginToAccount(scraperOptions);

            //Profile Page
            var profilePage = new ProfilePage(_driver, scraperOptions.TargetAccount);
            profilePage.GetProfilePicture(targetMedia);

            //Story Page
            if (scraperOptions.ScrapeStory)
                ScrapeStory(profilePage, targetMedia);
            
            var postPage = profilePage.EnterPosts(targetMedia);


            //PostPage
            postPage.GetPostData();

            Console.WriteLine("Total Program Time: " + (watch.ElapsedMilliseconds) / 1000.00 + " seconds");
        }

        private static void ExecuteScraper(ScraperOptions scraperOptions,
            ITargetBlock<KeyValuePair<string, string>> targetMedia,
            ITargetBlock<KeyValuePair<string, List<KeyValuePair<string, string>>>> targetText)
        {
            //Login
            var watch = Stopwatch.StartNew();
            if (!scraperOptions.Username.Equals(string.Empty))
                LoginToAccount(scraperOptions);

            //Profile Page
            var profilePage = new ProfilePage(_driver, scraperOptions.TargetAccount);
            profilePage.GetProfilePicture(targetMedia);
            profilePage.GetProfileText(targetText);

            //Story Page
            if (scraperOptions.ScrapeStory)
                ScrapeStory(profilePage, targetMedia);

            var postPage = profilePage.EnterPosts(targetMedia, targetText);


            //PostPage
            postPage.GetPostDataWithComments();

            Console.WriteLine("Total Program Time: " + (watch.ElapsedMilliseconds) / 1000.00 + " seconds");
        }

        private static void LoginToAccount(ScraperOptions scraperOptions)
        {
            new LoginPage(_driver).Login(scraperOptions.Username, scraperOptions.Password);
        }

        private static void ScrapeStory(ProfilePage profilePage, ITargetBlock<KeyValuePair<string, string>> targetMedia)
        {
            var storyPage = profilePage.EnterStory(targetMedia);
            storyPage?.SaveStoryContent();
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