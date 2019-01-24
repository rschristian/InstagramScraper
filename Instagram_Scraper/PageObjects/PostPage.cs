using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks.Dataflow;
using Instagram_Scraper.Utility;
using OpenQA.Selenium;

namespace Instagram_Scraper.PageObjects
{
    public class PostPage
    {
        private readonly WebDriverExtensions _webHelper;
        
        private List<string> _tempLinkList = new List<string>();

        private readonly ITargetBlock<KeyValuePair<string, string>> _target;
        
        private readonly IWebDriver _driver;

        public PostPage(IWebDriver driver, ITargetBlock<KeyValuePair<string, string>> target)
        {
            _webHelper = new WebDriverExtensions(driver);
            _target = target;
            _driver = driver;
        }

        private IWebElement MultiSrcPostChevron => _webHelper.SafeFindElement(".coreSpriteRightChevron");
        
        private IWebElement NextPostPaginationArrow => _webHelper.SafeFindElement(".coreSpriteRightPaginationArrow");
        
        private IEnumerable<IWebElement> ImageSourceClass => _webHelper.SafeFindElements(".kPFhm img");
        
        private IEnumerable<IWebElement> VideoSourceClass => _webHelper.SafeFindElements(".tWeCl");
        
        //TODO Issues with posts. Occasionally, mutli-src posts are skipped over entirely
        public void GetPostData()
        {
            try
            {
                _webHelper.WaitForElement(By.CssSelector(".eo2As"), 20);
    
                if (MultiSrcPostChevron != null)
                {
                    if (VideoSourceClass.Any())
                    {
                        foreach (var webElement in VideoSourceClass)
                        {
                            _tempLinkList.Add(webElement.GetAttribute("src"));
                        }
                    }
                    else if (ImageSourceClass.Any())
                    {
                        foreach (var webElement in ImageSourceClass)
                        {
                            var stringList = webElement.GetAttribute("srcset").Split(',');
                            var index = Array.FindIndex(stringList, row => row.Contains("1080w"));
                            _tempLinkList.Add(stringList[index].Remove(stringList[index].Length - 6));
                        }
                    }
    
                    MultiSrcPostChevron.Click();
                    GetPostData();
                }
                else
                {
                    if (VideoSourceClass.Any())
                    {
                        foreach (var webElement in VideoSourceClass)
                        {
                            _tempLinkList.Add(webElement.GetAttribute("src"));
                        }
                    }
                    else if (ImageSourceClass.Any())
                    {
                        foreach (var webElement in ImageSourceClass)
                        {
                            var stringList = webElement.GetAttribute("srcset").Split(',');
                            var index = Array.FindIndex(stringList, row => row.Contains("1080w"));
                            _tempLinkList.Add(stringList[index].Remove(stringList[index].Length - 6));
                        }
                    }
    
                    _tempLinkList = _tempLinkList.Distinct().ToList();
                    var timeStamp = _webHelper.RefineTimeStamp();
    
                    for (var i = 0; i < _tempLinkList.Count; i++)
                    {
                        _target.Post(new KeyValuePair<string, string>(timeStamp + " " + (_tempLinkList.Count - i), 
                            _tempLinkList[i]));
                    }
    
                    if (NextPostPaginationArrow != null)
                    {
                        NextPostPaginationArrow.Click();
                        _tempLinkList.Clear();
                        new PostPage(_driver, _target).GetPostData();
                    }
                    else
                    {
                        _target.Complete();
                        Console.WriteLine("Finished");
                    }
                }
            }
            catch (StaleElementReferenceException)
            {
                Console.WriteLine("Stale Element, Retrying");
                GetPostData();
            }
        }
    }
}