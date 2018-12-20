using OpenQA.Selenium;
using SeleniumExtras.PageObjects;
using How = OpenQA.Selenium.Support.PageObjects.How;

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
        
        [OpenQA.Selenium.Support.PageObjects.FindsBy(How = How.CssSelector, Using = "div._bz0w a")]
        private IWebElement firstPost;
        
        [OpenQA.Selenium.Support.PageObjects.FindsBy(How = How.CssSelector, Using = "._6q-tv")]
        private IWebElement profilePicture;
        
        [OpenQA.Selenium.Support.PageObjects.FindsBy(How = How.CssSelector, Using = "div.RR-M-")]
        private IWebElement storyClass;

        public void goToProfile(string targetAccount)
        {
            _driver.Navigate().GoToUrl("http://www.instagram.com/" + targetAccount + "/");
        }

        public StoryPage enterStory()
        {
            var executor = (IJavaScriptExecutor) _driver;
            executor.ExecuteScript("arguments[0].click();", firstPost);
            return new StoryPage(_driver);
        }
    }
}