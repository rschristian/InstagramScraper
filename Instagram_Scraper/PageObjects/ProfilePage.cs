using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks.Dataflow;
using Instagram_Scraper.Utility;
using OpenQA.Selenium;

namespace Instagram_Scraper.PageObjects
{
    public class ProfilePage
    {
        private readonly IWebDriver _driver;

        private readonly WebDriverExtensions _webHelper;

        public ProfilePage(IWebDriver driver, string targetAccount)
        {
            _driver = driver;
            _webHelper = new WebDriverExtensions(driver);
            _driver.Navigate().GoToUrl("http://www.instagram.com/" + targetAccount + "/");
        }

        private IWebElement FirstPost => _driver.FindElement(By.CssSelector("div._bz0w a"));

        private IWebElement ProfilePicture => _driver.FindElement(By.CssSelector("._6q-tv"));

        private IWebElement Story => _webHelper.SafeFindElement(".h5uC0");
        
        private IWebElement ProfileText => _webHelper.SafeFindElement(".-vDIg span");

        public void GetProfilePicture(ITargetBlock<KeyValuePair<string, string>> targetMedia)
        {
            targetMedia.Post(new KeyValuePair<string, string>(DateTime.Now.ToString("yyyy-MM-dd") + " profile",
                ProfilePicture.GetAttribute("src")));
        }

        public StoryPage EnterStory(ITargetBlock<KeyValuePair<string, string>> targetMedia)
        {
            Thread.Sleep(500);
            if (Story == null) return null;
            Console.WriteLine("Account has a story currently");
            Story.Click();
            return new StoryPage(_driver, targetMedia);
        }

        public void GetProfileText(ITargetBlock<KeyValuePair<string, List<KeyValuePair<string, string>>>> targetText)
        {
            var profileText = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>(DateTime.Now.ToString("yyyy-MM-dd"), ProfileText.Text)
            };
            targetText.Post(new KeyValuePair<string, List<KeyValuePair<string, string>>>("Profile", profileText)); 
        }

        public PostPage EnterPosts(ITargetBlock<KeyValuePair<string, string>> target)
        {
            FirstPost.Click();
            return new PostPage(_driver, target);
        }

        public PostPage EnterPosts(ITargetBlock<KeyValuePair<string, string>> targetMedia,
            ITargetBlock<KeyValuePair<string, List<KeyValuePair<string, string>>>> targetText)
        {
            FirstPost.Click();
            return new PostPage(_driver, targetMedia, targetText);
        }
    }
}