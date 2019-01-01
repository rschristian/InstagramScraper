using System;
using System.Collections.Generic;
using System.IO;
using OpenQA.Selenium;
using SeleniumExtras.PageObjects;
using System.Net;
using System.Threading.Tasks.Dataflow;
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
        
        public void GetProfilePicture(ITargetBlock<KeyValuePair<string, string>> target)
        {
            target.SendAsync(new KeyValuePair<string, string>(DateTime.Now.ToString("yyyy-M-d") + " profile", ProfilePicture.GetAttribute("src")));
        }

        public StoryPage EnterStory(Queue<KeyValuePair<string, string>> downloadQueue)
        {
            _webHelper.FindElement(By.CssSelector("div.RR-M-"), 5);
            if (StoryClass == null) return null;
            var executor = (IJavaScriptExecutor) _driver;
            executor.ExecuteScript("arguments[0].click();", StoryClass);
            return new StoryPage(_driver, downloadQueue);
        }
        
        public PostPage EnterPosts(ITargetBlock<KeyValuePair<string, string>> target)
        {
            var executor = (IJavaScriptExecutor) _driver;
            executor.ExecuteScript("arguments[0].click();", FirstPost);
            return new PostPage(_driver, target);
        }
    }
}