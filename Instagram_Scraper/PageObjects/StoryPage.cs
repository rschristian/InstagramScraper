using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks.Dataflow;
using Instagram_Scraper.Utility;
using OpenQA.Selenium;

namespace Instagram_Scraper.PageObjects
{
    public class StoryPage
    {
        private readonly WebDriverExtensions _webHelper;
        
        private readonly List<string> _tempLinkList = new List<string>();
        
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
            _webHelper.WaitForElement(By.CssSelector("._7zQEa"), 2000);

            if (StoryVideoSrcClass.Any())
                _tempLinkList.Add(StoryVideoSrcClass.First().GetAttribute("src"));
            else if (StoryImageSrcClass.Any())
                foreach (var webElement in StoryImageSrcClass)
                {
                    var stringList = webElement.GetAttribute("srcset").Split(',');
                    var index = Array.FindIndex(stringList, row => row.Contains("1080w"));
                    _tempLinkList.Add(stringList[index].Remove(stringList[index].Length - 6));
                }

            if (_tempLinkList.Count < StoryPageNavigationClass.Count())
            {
                StoryChevronClass.Click();
                SaveStoryContent();
            }
            else
            {
                var storiesProcessedCount = 0;
                var currentDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                foreach (var link in _tempLinkList)
                {
                    _target.Post(new KeyValuePair<string, string>(currentDateTime + " story " +
                                                                  (_tempLinkList.Count - storiesProcessedCount), link));
                    storiesProcessedCount++;
                }

                Console.WriteLine("Story Capture Complete");
                StoryChevronClass.Click();
            }
        }
    }
}