using System;
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

        public ProfilePage(IWebDriver driver)
        {
            _driver = driver;
            PageFactory.InitElements(driver, this);
        }
        
        private IWebElement FirstPost => _driver.FindElement(By.CssSelector("div._bz0w a"));
        
        private IWebElement ProfilePicture => _driver.FindElement(By.CssSelector("._6q-tv"));
        
        private IWebElement StoryClass => _driver.FindElement(By.CssSelector("div.RR-M-"));
        

        public void GoToProfile(string targetAccount)
        {
            _driver.Navigate().GoToUrl("http://www.instagram.com/" + targetAccount + "/");
            
        }
        
        public void GetProfilePicture(UriNameDictionary resourcesDictionary)
        {
            resourcesDictionary.Add(DateTime.Now.ToString("yyyy-M-d") + " profile", ProfilePicture.GetAttribute("src"));
        }

        public StoryPage EnterStory()
        {
            var executor = (IJavaScriptExecutor) _driver;
            executor.ExecuteScript("arguments[0].click();", StoryClass);
            return new StoryPage(_driver);
        }
        
        public PostPage EnterPosts()
        {
            var executor = (IJavaScriptExecutor) _driver;
            executor.ExecuteScript("arguments[0].click();", FirstPost);
            return new PostPage(_driver);
        }
    }
}