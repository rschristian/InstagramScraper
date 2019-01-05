using System.Collections.Generic;
using System.Threading.Tasks.Dataflow;
using OpenQA.Selenium;
using Selenium.Utility;

namespace Selenium.PageObjects
{
    public class StoryPage
    {
        private readonly WebDriverExtensions _webHelper;
        
        private readonly ITargetBlock<KeyValuePair<string, string>> _target;

        public StoryPage(IWebDriver driver, ITargetBlock<KeyValuePair<string, string>> target)
        {
            _webHelper = new WebDriverExtensions(driver);
            _target = target;
        }
        
        private IWebElement StoryVideoSrcClass => _webHelper.SafeFindElement(".OFkrO source");
        
        //This will always exist, as for a video, the thumbnail is stored here
        private IWebElement StoryImageSrcClass => _webHelper.SafeFindElement("._7NpAS");

        public void SaveStoryContent()
        {
            //get story here
        }
        
    }
}