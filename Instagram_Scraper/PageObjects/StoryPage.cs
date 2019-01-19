using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks.Dataflow;
using OpenQA.Selenium;
using Selenium.Utility;

namespace Selenium.PageObjects
{
    public class StoryPage
    {
        private readonly WebDriverExtensions _webHelper;
        
        private List<string> _tempLinkList = new List<string>();
        
        private readonly ITargetBlock<KeyValuePair<string, string>> _target;

        public StoryPage(IWebDriver driver, ITargetBlock<KeyValuePair<string, string>> target)
        {
            _webHelper = new WebDriverExtensions(driver);
            _target = target;
        }
            
        private IWebElement StoryChevronClass => _webHelper.SafeFindElement(".ow3u_");
        
        private IEnumerable<IWebElement> StoryVideoSrcClass => _webHelper.SafeFindElements(".OFkrO source");
        
        private IEnumerable<IWebElement> StoryImageSrcClass => _webHelper.SafeFindElements("._7NpAS");
        
        private IEnumerable<IWebElement> StoryPageNavigationClass => _webHelper.SafeFindElements("._7zQEa");

        public void SaveStoryContent()
        {
            _webHelper.WaitForElement(By.CssSelector("._7zQEa"), 2);
                
            if (StoryVideoSrcClass.Any())
            {
                _tempLinkList.Add(StoryVideoSrcClass.First().GetAttribute("src"));
            }
            else if (StoryImageSrcClass.Any())
            {
                foreach (var webElement in StoryImageSrcClass)
                {
                    var stringList = webElement.GetAttribute("srcset").Split(',');
                    var index = Array.FindIndex(stringList, row => row.Contains("1080w"));
                    _tempLinkList.Add(stringList[index].Remove(stringList[index].Length - 6));
                }
            }

            if (_tempLinkList.Count < StoryPageNavigationClass.Count())
            {
                StoryChevronClass.Click();
                SaveStoryContent();
            }
            else
            {
                for (var i = 0; i < _tempLinkList.Count; i++)
                {
                    _target.Post(new KeyValuePair<string, string>(DateTime.Now.ToString("yyyy-MM-d") + " story " 
                                                                                                     + (_tempLinkList.Count - i), _tempLinkList[i]));
                }
            
                Console.WriteLine("Story Capture Complete");
                StoryChevronClass.Click();
            }
        }
    }
}