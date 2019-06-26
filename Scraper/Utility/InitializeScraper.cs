using System;
using System.IO;
using Instagram_Scraper.Controller;
using Instagram_Scraper.Domain;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;

namespace Instagram_Scraper.Utility
{
    public static class InitializeScraper
    {
        private static IWebDriver _driver;

        public static async void SetUp(ScraperOptions scraperOptions)
        {
            var optionsChrome = new ChromeOptions();
            optionsChrome.AddUserProfilePreference("profile.default_content_setting_values.images", 2);
            optionsChrome.AddArguments("--disable-popup-blocking", "--window-size=1920,1080", "--mute-audio");

            if (scraperOptions.Headless) optionsChrome.AddArgument("headless");
            _driver = new ChromeDriver("./bin/Debug/netcoreapp2.2", optionsChrome);


            string savePath;
            var homePath = Environment.OSVersion.Platform == PlatformID.Unix ||
                           Environment.OSVersion.Platform == PlatformID.MacOSX
                ? Environment.GetEnvironmentVariable("HOME")
                : Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");
            if (scraperOptions.FolderSavePath.Equals(string.Empty))
                savePath = homePath + "/Pictures/" + scraperOptions.TargetAccount + "/";
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


            if (!scraperOptions.OnlyScrapeStory)
            {
                var bufferMedia = WebDriverExtensions.StartMediaService(savePath);
                var bufferStory = scraperOptions.ScrapeStory ? WebDriverExtensions.StartStoryService(savePath) : null;
                var bufferText = scraperOptions.ScrapeComments ? WebDriverExtensions.StartTextService(savePath) : null;
                new ScraperController(_driver, scraperOptions, bufferMedia, bufferText, bufferStory).ExecuteScraper();
                
                await bufferMedia.Completion;
                if (bufferText != null) await bufferText.Completion;
                if (bufferStory != null) await bufferStory.Completion;
            }
            else
            {
                var bufferStory = WebDriverExtensions.StartStoryService(savePath);
                new ScraperController(_driver, scraperOptions, null, null, bufferStory).OnlyScrapeStory();
                await bufferStory.Completion;
            }
            _driver.Quit();
        }
    }
}