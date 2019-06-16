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
                _driver = new ChromeDriver("./bin/Debug/netcoreapp2.2", optionsChrome);
                
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


            if (!scraperOptions.OnlyScrapeStory)
            {
                var bufferMedia = WebDriverExtensions.StartMediaService(savePath);
                
                //Just media
                if (!scraperOptions.ScrapeComments && !scraperOptions.ScrapeStory)
                {
                    new ScraperController(_driver, scraperOptions, bufferMedia).ExecuteScraper();
                    await bufferMedia.Completion;
                }
                //Media + Story
                else if (!scraperOptions.ScrapeComments && scraperOptions.ScrapeStory)
                {
                    var bufferStory = WebDriverExtensions.StartStoryService(savePath);
                    new ScraperController(_driver, scraperOptions, bufferMedia, null, bufferStory).ExecuteScraper();
                    await bufferMedia.Completion;
                    await bufferStory.Completion;
                }
                //Media + Comments
                else if (scraperOptions.ScrapeComments && !scraperOptions.ScrapeStory)
                {
                    var bufferText = WebDriverExtensions.StartTextService(savePath);
                    new ScraperController(_driver, scraperOptions, bufferMedia, bufferText).ExecuteScraper();
                    await bufferMedia.Completion;
                    await bufferText.Completion; 
                }
                //Media + Comments + Story
                else 
                {
                    var bufferText = WebDriverExtensions.StartTextService(savePath);
                    var bufferStory = WebDriverExtensions.StartStoryService(savePath);
                    new ScraperController(_driver, scraperOptions, bufferMedia, bufferText, bufferStory).ExecuteScraper();
                    await bufferMedia.Completion;
                    await bufferText.Completion; 
                    await bufferStory.Completion;
                }
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