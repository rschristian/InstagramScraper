using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks.Dataflow;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Instagram_Scraper.Utility
{
    public class WebDriverExtensions
    {
        private readonly IWebDriver _driver;

        public WebDriverExtensions(IWebDriver driver)
        {
            _driver = driver;
        }

        public IWebElement WaitForElement(By by, int timeoutInMilliSeconds)
        {
            try
            {
                if (timeoutInMilliSeconds <= 0) _driver.FindElement(by);
                var wait = new WebDriverWait(_driver, TimeSpan.FromMilliseconds(timeoutInMilliSeconds));
                return wait.Until(drv => drv.FindElement(by));
            }
            catch (WebDriverTimeoutException)
            {
                return null;
            }
        }

        public IWebElement SafeFindElement(string selector)
        {
            try
            {
                return _driver.FindElement(By.CssSelector(selector));
            }
            catch (NoSuchElementException)
            {
                return null;
            }
        }

        public IEnumerable<IWebElement> SafeFindElements(string selector)
        {
            return _driver.FindElements(By.CssSelector(selector));
        }

        public string RefineTimeStamp()
        {
            var timeStamp = SafeFindElement("time[datetime]").GetAttribute("datetime");
            timeStamp = timeStamp.Substring(0, 10) + " " + timeStamp.Substring(11, 8);
            return timeStamp;
        }

        public static BufferBlock<KeyValuePair<string, string>> StartMediaService(string savePath)
        {
            var bufferMedia = new BufferBlock<KeyValuePair<string, string>>();
            var backgroundThreadMedia = new Thread(() => DownloadManager.ConsumeMediaAsync(savePath, bufferMedia))
                {
                    IsBackground = true
                };
            backgroundThreadMedia.Start();
            return bufferMedia;
        }
        
        public static BufferBlock<KeyValuePair<string, List<KeyValuePair<string, string>>>> StartTextService(string savePath)
        {
            var bufferText = new BufferBlock<KeyValuePair<string, List<KeyValuePair<string, string>>>>();
            var backgroundThreadText = new Thread(() => DownloadManager.ConsumeTextAsync(savePath, bufferText))
                {
                    IsBackground = true
                };
            backgroundThreadText.Start(); 
            return bufferText;
        }
        
        public static BufferBlock<KeyValuePair<string, string>> StartStoryService(string savePath)
        {
            var bufferStory = new BufferBlock<KeyValuePair<string, string>>();
            var backgroundThreadStory = new Thread(() => DownloadManager.ConsumeStoryAsync(savePath, bufferStory))
                {
                    IsBackground = true
                };
            backgroundThreadStory.Start();
            return bufferStory;
        }

        public static List<KeyValuePair<string, byte[]>> GetFilesFromDirectory(string path)
        {
            var fileInfo = new DirectoryInfo(path)
                .GetFiles(DateTime.Now.ToString("yyyy-MM-dd") + "*");
            return fileInfo
                .Select(file => new KeyValuePair<string, byte[]>(file.Name,
                    File.ReadAllBytes(path + file.Name)))
                .OrderByDescending(x => x.Key)
                .ToList();
        }
    }
}