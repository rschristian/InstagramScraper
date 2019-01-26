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

        public ProfilePage(IWebDriver driver)
        {
            _driver = driver;
            _webHelper = new WebDriverExtensions(driver);
        }

        private IWebElement FirstPost => _driver.FindElement(By.CssSelector("div._bz0w a"));

        private IWebElement ProfilePicture => _driver.FindElement(By.CssSelector("._6q-tv"));

        private IWebElement StoryClass => _webHelper.SafeFindElement(".h5uC0");

        public void GoToProfile(string targetAccount)
        {
            _driver.Navigate().GoToUrl("http://www.instagram.com/" + targetAccount + "/");
        }

        public void GetProfilePicture(ITargetBlock<KeyValuePair<string, string>> target)
        {
            target.SendAsync(new KeyValuePair<string, string>(DateTime.Now.ToString("yyyy-MM-d") + " profile",
                ProfilePicture.GetAttribute("src")));
        }

        public StoryPage EnterStory(ITargetBlock<KeyValuePair<string, string>> target)
        {
            Thread.Sleep(500);
            if (StoryClass == null)
            {
                Console.WriteLine("No Story");
                return null;
            }

            Console.WriteLine("There is a story");
            Thread.Sleep(750);
            StoryClass.Click();
            return new StoryPage(_driver, target);
        }

        public PostPage EnterPosts(ITargetBlock<KeyValuePair<string, string>> target)
        {
            _webHelper.WaitForElement(By.CssSelector("div._bz0w a"), 5000);
            var executor = (IJavaScriptExecutor) _driver;
            executor.ExecuteScript("arguments[0].click();", FirstPost);
            return new PostPage(_driver, target);
        }

        public PostPage EnterPosts(ITargetBlock<KeyValuePair<string, string>> targetMedia,
            ITargetBlock<KeyValuePair<string, List<KeyValuePair<string, string>>>> targetText)
        {
            _webHelper.WaitForElement(By.CssSelector("div._bz0w a"), 5000);
            var executor = (IJavaScriptExecutor) _driver;
            executor.ExecuteScript("arguments[0].click();", FirstPost);
            return new PostPage(_driver, targetMedia, targetText);
        }
    }
}