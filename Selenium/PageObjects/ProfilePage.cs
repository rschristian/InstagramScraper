using System;
using System.Collections.Generic;
using System.IO;
using OpenQA.Selenium;
using SeleniumExtras.PageObjects;
using System.Net;
using Selenium.Utility;

namespace Selenium.PageObjects
{
    public class ProfilePage
    {
        private readonly IWebDriver _driver;
        
        private readonly WebDriverExtensions _webHelper;

        public ProfilePage(IWebDriver driver)
        {
            _driver = driver;
            _webHelper = new WebDriverExtensions(driver);
        }
        
        private IWebElement FirstPost => _driver.FindElement(By.CssSelector("div._bz0w a"));
        
        private IWebElement ProfilePicture => _driver.FindElement(By.CssSelector("._6q-tv"));
        
        private IWebElement StoryClass => _driver.FindElement(By.CssSelector("div.RR-M-"));
        

        public void GoToProfile(string targetAccount)
        {
            _driver.Navigate().GoToUrl("http://www.instagram.com/" + targetAccount + "/");
            
        }
        
        public void GetProfilePicture(Queue<KeyValuePair<string, string>> downloadQueue)
        {
            downloadQueue.Enqueue(new KeyValuePair<string, string>(DateTime.Now.ToString("yyyy-M-d") + " profile", ProfilePicture.GetAttribute("src")));
        }

        public StoryPage EnterStory()
        {
            _webHelper.FindElement(By.CssSelector("div.RR-M-"), 5);
            if (!WebDriverExtensions.IsElementPresent(StoryClass)) return null;
            var executor = (IJavaScriptExecutor) _driver;
            executor.ExecuteScript("arguments[0].click();", StoryClass);
            return new StoryPage(_driver);
        }
        
        public PostPage EnterPosts(string fileSavePath, Queue<KeyValuePair<string, string>> downloadQueue)
        {
            var executor = (IJavaScriptExecutor) _driver;
            executor.ExecuteScript("arguments[0].click();", FirstPost);
            return new PostPage(_driver, fileSavePath, downloadQueue);
        }
    }
}