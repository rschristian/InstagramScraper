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
            PageFactory.InitElements(driver, this);
        }
        
        [OpenQA.Selenium.Support.PageObjects.FindsBy(How = How.CssSelector, Using = ".OFkrO source")]
        private IWebElement storyVideoSrcClass;
        
        //This will always exist, as for a video, the thumbnail is stored here
        [OpenQA.Selenium.Support.PageObjects.FindsBy(How = How.CssSelector, Using = "._7NpAS")]
        private IWebElement storyImageSrcClass;

        public void SaveStoryContent()
        {
            //get story here
        }
        
    }
}