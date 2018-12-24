using OpenQA.Selenium;
using SeleniumExtras.PageObjects;
using How = OpenQA.Selenium.Support.PageObjects.How;

namespace Selenium.PageObjects
{
    public class StoryPage
    {
        private readonly IWebDriver _driver;

        public StoryPage(IWebDriver driver)
        {
            _driver = driver;
        }
        
        private IWebElement StoryVideoSrcClass => _driver.FindElement(By.CssSelector(".OFkrO source"));
        
        //This will always exist, as for a video, the thumbnail is stored here
        private IWebElement StoryImageSrcClass => _driver.FindElement(By.CssSelector("._7NpAS"));

        public void SaveStoryContent()
        {
            //get story here
        }
        
    }
}