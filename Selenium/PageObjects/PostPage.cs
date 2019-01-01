using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks.Dataflow;
using OpenQA.Selenium;
using Selenium.Utility;

namespace Selenium.PageObjects
{
    public class PostPage
    {
        private readonly WebDriverExtensions _webHelper;
        
        private List<string> _tempLinkList = new List<string>();

        private readonly ITargetBlock<KeyValuePair<string, string>> _target;
        
        private UriNameDictionary resourceDictionary = new UriNameDictionary();

        public PostPage(IWebDriver driver, ITargetBlock<KeyValuePair<string, string>> target)
        {
            _webHelper = new WebDriverExtensions(driver);
            _target = target;
        }

        private IWebElement MultiSrcPostChevron => _webHelper.SafeFindElement(".coreSpriteRightChevron");
        
        private IWebElement NextPostPaginationArrow => _webHelper.SafeFindElement(".coreSpriteRightPaginationArrow");
        
        private IWebElement PostTimeStamp => _webHelper.SafeFindElement("time[datetime]");
        
        private IEnumerable<IWebElement> ImageSourceClass => _webHelper.SafeFindElements(".kPFhm img");
        
        private IEnumerable<IWebElement> VideoSourceClass => _webHelper.SafeFindElements(".tWeCl");
        

        public void GetPostData()
        {
            try
            {
                _webHelper.FindElement(By.CssSelector(".eo2As"), 5);

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
                    var timeStamp = RefineTimeStamp();

                    for (var i = 0; i < _tempLinkList.Count; i++)
                    {
                        resourceDictionary.Add(timeStamp + " " + (_tempLinkList.Count - i), _tempLinkList[i]);
                        _target.Post(new KeyValuePair<string, string>(timeStamp + " " + (_tempLinkList.Count - i), 
                            _tempLinkList[i]));
                    }

                    if (WebDriverExtensions.IsElementPresent(NextPostPaginationArrow))
                    {
                        NextPostPaginationArrow.Click();
                        _tempLinkList.Clear();
                        GetPostData();
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

        private string RefineTimeStamp()
        {
            var timeStamp = PostTimeStamp.GetAttribute("datetime");
            timeStamp = timeStamp.Substring(0, 10) + " " + timeStamp.Substring(12, 7);
            return timeStamp;
        }

        // private void DownloadFile()
        // {
        //     if(!File.Exists(_path)) {Directory.CreateDirectory(_path);}
        //     var client = new WebClient();
        //     
        //     while (!_downloadQueue.Any()) return;
        //     
        //     var url = _downloadQueue.Dequeue();
        //
        //     if (File.Exists(_path + url.Key + ".*")) return;
        //     if (url.Value.Contains(".mp4"))
        //     {
        //         client.DownloadFileAsync(new Uri(url.Value), _path + url.Key + ".mp4");
        //     }
        //     else
        //     {
        //         client.DownloadFileAsync(new Uri(url.Value), _path + url.Key + ".jpg");
        //     }
        // }
    }
}