using System.Collections.Generic;
using OpenQA.Selenium;

namespace Selenium.PageObjects
{
    public class StoryPage
    {
        private readonly IWebDriver _driver;

        private readonly Queue<KeyValuePair<string, string>> _downloadQueue;

        public StoryPage(IWebDriver driver, Queue<KeyValuePair<string, string>> downloadQueue)
        {
            _driver = driver;
            _downloadQueue = downloadQueue;
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