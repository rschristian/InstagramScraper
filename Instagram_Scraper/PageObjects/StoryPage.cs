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
        
        private readonly ITargetBlock<KeyValuePair<string, string>> _target;

        private readonly Dictionary<string, string> _tempDictionary = new Dictionary<string, string>();

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
                _tempDictionary.Add(StoryVideoSrcClass.First().GetAttribute("src"),
                    _webHelper.RefineDateStamp());
            else if (StoryImageSrcClass.Any())
                foreach (var webElement in StoryImageSrcClass)
                {
                    var stringList = webElement.GetAttribute("srcset").Split(',');
                    var index = Array.FindIndex(stringList, row => row.Contains("1080w"));
                    _tempDictionary.Add(stringList[index].Remove(stringList[index].Length - 6),
                        _webHelper.RefineDateStamp());
                }

            if (_tempDictionary.Count < StoryPageNavigationClass.Count())
            {
                StoryChevronClass.Click();
                SaveStoryContent();
            }
            else
            {
                var i = 0;
                var currentTime = DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second;
                foreach (var (link, timestamp) in _tempDictionary)
                {
                    _target.Post(new KeyValuePair<string, string>(timestamp + " " + currentTime + " story " +
                                                                  (_tempDictionary.Count - i), link));
                    i++;
                }

                Console.WriteLine("Story Capture Complete");
                StoryChevronClass.Click();
            }
        }
    }
}