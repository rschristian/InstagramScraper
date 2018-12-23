using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using OpenQA.Selenium;
using Selenium.Utility;

namespace Selenium.PageObjects
{
    public class PostPage
    {
        private readonly IWebDriver _driver;

        private int _contentInSet = 1;

        private readonly WebDriverExtensions _webHelper;
        
        private List<string> _tempLinkList = new List<string>();

        public PostPage(IWebDriver driver)
        {
            _driver = driver;
            _webHelper = new WebDriverExtensions(driver);
        }
                                                                                                                                
        private IWebElement MultiSrcPostChevronRoot => _webHelper.SafeFindElement("._97aPb");

        private IWebElement MultiSrcPostChevron => _webHelper.SafeFindElement(".coreSpriteRightChevron");

        private IWebElement NextPostPaginationArrow => _webHelper.SafeFindElement(".coreSpriteRightPaginationArrow");
        
        private IWebElement PostTimeStamp => _webHelper.SafeFindElement("time[datetime]");

        private IEnumerable<IWebElement> ImageSourceClass => _webHelper.SafeFindElements(".kPFhm img");

        private IEnumerable<IWebElement> VideoSourceClass => _webHelper.SafeFindElements(".tWeCl");
        

        public void GetPostData(UriNameDictionary resourceDictionary)
        {
            try
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();
                _webHelper.FindElement(By.CssSelector(".kPFhm img"), 2);
                watch.Stop();
                Console.WriteLine("Time spent waiting: " + watch.ElapsedMilliseconds);
                //Console.WriteLine("URL of Page: " + _driver.Url);

                if (WebDriverExtensions.IsElementPresent(MultiSrcPostChevron))
                {
                    _contentInSet++;
                    if (WebDriverExtensions.IsElementsPresent(VideoSourceClass))
                    {
                        foreach (var webElement in VideoSourceClass)
                        {
                            _tempLinkList.Add(webElement.GetAttribute("src"));
                        }
                    }
                    else if (WebDriverExtensions.IsElementsPresent(ImageSourceClass))
                    {
                        foreach (var webElement in ImageSourceClass)
                        {
                            if (!_webHelper.IsElementVisible(webElement)) continue;
                            var stringList = webElement.GetAttribute("srcset").Split(',');
                            var index = Array.FindIndex(stringList, row => row.Contains("1080w"));
                            _tempLinkList.Add(stringList[index].Remove(stringList[index].Length - 6));
                        }
                    }

                    MultiSrcPostChevron.Click();
                    GetPostData(resourceDictionary);
                }
                else
                {
                    if (WebDriverExtensions.IsElementsPresent(VideoSourceClass))
                    {
                        foreach (var webElement in VideoSourceClass)
                        {
                            _tempLinkList.Add(webElement.GetAttribute("src"));
                        }
                    }
                    else if (WebDriverExtensions.IsElementsPresent(ImageSourceClass))
                    {
                        foreach (var webElement in ImageSourceClass)
                        {
                            if (!_webHelper.IsElementVisible(webElement)) continue;
                            var stringList = webElement.GetAttribute("srcset").Split(',');
                            var index = Array.FindIndex(stringList, row => row.Contains("1080w"));
                            _tempLinkList.Add(stringList[index].Remove(stringList[index].Length - 6));
                        }
                    }

                    _tempLinkList = _tempLinkList.Distinct().ToList();
                    var timeStamp = RefineTimeStamp();

                    for (var i = 0; i < _tempLinkList.Count; i++)
                    {
                        resourceDictionary.Add(timeStamp + " " + (_tempLinkList.Count - i), _tempLinkList[i]);
                    }

                    if (WebDriverExtensions.IsElementPresent(NextPostPaginationArrow))
                    {
                        NextPostPaginationArrow.Click();
                        _contentInSet = 1;
                        _tempLinkList.Clear();
                        GetPostData(resourceDictionary);
                    }
                    else
                    {
                        Console.WriteLine("Finished");
                        //finish
                    }
                }
            }
            catch (StaleElementReferenceException)
            {
                Console.WriteLine("FUCK");
                GetPostData(resourceDictionary);
            }
        }

        private string RefineTimeStamp()
        {
            var timeStamp = PostTimeStamp.GetAttribute("datetime");
            timeStamp = timeStamp.Substring(0, 10) + " " + timeStamp.Substring(12, 7);
            return timeStamp;
        }
    }
}