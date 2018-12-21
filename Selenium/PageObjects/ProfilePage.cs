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
        private readonly WebClient _webClient = new WebClient();

        private readonly UriNameDictionary _resourcesDictionary = new UriNameDictionary();

        private string _path = "/home/ryun/Pictures/";

        public ProfilePage(IWebDriver driver)
        {
            _driver = driver;
            PageFactory.InitElements(driver, this);
        }
        
        [FindsBy(How = How.CssSelector, Using = "div._bz0w a")]
        private IWebElement _firstPost;
        
        [FindsBy(How = How.CssSelector, Using = "._6q-tv")]
        private IWebElement _profilePicture;
        
        [FindsBy(How = How.CssSelector, Using = "div.RR-M-")]
        private IWebElement _storyClass;
        

        public void GoToProfile(string targetAccount)
        {
            _path = _path + targetAccount + "/";
            _driver.Navigate().GoToUrl("http://www.instagram.com/" + targetAccount + "/");
            
        }
        
        public void GetProfilePicture()
        {
            _resourcesDictionary.Add(DateTime.Now.ToString("yyyy-M-d") + " profile", _profilePicture.GetAttribute("src"));
            
//            foreach (var entry in _resourcesDictionary)
//            {
//                Directory.CreateDirectory(_path);
//                _webClient.DownloadFile(entry.Value, _path + entry.Key + ".png");
//            }
        }

        public StoryPage EnterStory()
        {
            var executor = (IJavaScriptExecutor) _driver;
            executor.ExecuteScript("arguments[0].click();", _storyClass);
            return new StoryPage(_driver);
        }
        
        public PostPage EnterPosts()
        {
            var executor = (IJavaScriptExecutor) _driver;
            executor.ExecuteScript("arguments[0].click();", _firstPost);
            return new PostPage(_driver);
        }
    }
}