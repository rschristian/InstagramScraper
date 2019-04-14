using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks.Dataflow;
using Instagram_Scraper.Utility;
using NLog;
using OpenQA.Selenium;

namespace Instagram_Scraper.PageObjects
{
    public class PostPage
    {
        private static readonly Logger Logger = LogManager.GetLogger("Post Page");
        
        private readonly IWebDriver _driver;
        
        private readonly WebDriverExtensions _webHelper;

        private List<string> _tempLinkList = new List<string>();
        
        private readonly ITargetBlock<KeyValuePair<string, string>> _targetMedia;

        private readonly ITargetBlock<KeyValuePair<string, List<KeyValuePair<string, string>>>> _targetText;

        public PostPage(IWebDriver driver, ITargetBlock<KeyValuePair<string, string>> targetMediaMedia)
        {
            _webHelper = new WebDriverExtensions(driver);
            _targetMedia = targetMediaMedia;
            _driver = driver;
        }

        public PostPage(IWebDriver driver, ITargetBlock<KeyValuePair<string, string>> targetMediaMedia,
            ITargetBlock<KeyValuePair<string, List<KeyValuePair<string, string>>>> targetText)
        {
            _webHelper = new WebDriverExtensions(driver);
            _targetMedia = targetMediaMedia;
            _driver = driver;
            _targetText = targetText;
        }

        private IWebElement MultiSrcPostChevron => _webHelper.SafeFindElement(".coreSpriteRightChevron");

        private IWebElement NextPostPaginationArrow => _webHelper.SafeFindElement(".coreSpriteRightPaginationArrow");

        private IEnumerable<IWebElement> ImageSourceClass => _webHelper.SafeFindElements(".FFVAD");

        private IEnumerable<IWebElement> VideoSourceClass => _webHelper.SafeFindElements(".tWeCl");
        
        private IWebElement ViewAllCommentsClass => _webHelper.SafeFindElement(".lnrre");
        
        private IEnumerable<IWebElement> CommentUserList => _webHelper.SafeFindElements(".FPmhX").ToList();
        
        private IEnumerable<IWebElement> CommentTextList => _webHelper.SafeFindElements("div.C4VMK span").ToList();

        public void GetPostData()
        {
            try
            {
                _webHelper.WaitForElement(By.CssSelector(".eo2As"), 2000);

                if (MultiSrcPostChevron != null)
                {
                    if (VideoSourceClass.Any())
                        foreach (var webElement in VideoSourceClass)
                            _tempLinkList.Add(webElement.GetAttribute("src"));
                    else if (ImageSourceClass.Any())
                        foreach (var webElement in ImageSourceClass)
                        {
                            if(!webElement.GetAttribute("srcset").Contains("1080w")) continue;
                            var stringList = webElement.GetAttribute("srcset").Split(',');
                            var index = Array.FindIndex(stringList, row => row.Contains("1080w"));
                            _tempLinkList.Add(stringList[index].Remove(stringList[index].Length - 6));
                        }
                    else
                        Logger.Debug("Neither video nor image content detected in post");

                    MultiSrcPostChevron.Click();
                    GetPostData();
                }
                else
                {
                    if (VideoSourceClass.Any())
                        foreach (var webElement in VideoSourceClass)
                            _tempLinkList.Add(webElement.GetAttribute("src"));
                    else if (ImageSourceClass.Any())
                        foreach (var webElement in ImageSourceClass)
                        {
                            if(!webElement.GetAttribute("srcset").Contains("1080w")) continue;
                            var stringList = webElement.GetAttribute("srcset").Split(',');
                            var index = Array.FindIndex(stringList, row => row.Contains("1080w"));
                            _tempLinkList.Add(stringList[index].Remove(stringList[index].Length - 6));
                        }
                    else
                        Logger.Debug("Neither video nor image content detected in post");

                    _tempLinkList = _tempLinkList.Distinct().ToList();
                    var timeStamp = _webHelper.RefineTimeStamp();

                    for (var i = 0; i < _tempLinkList.Count; i++)
                        _targetMedia.Post(new KeyValuePair<string, string>(timeStamp + " " + (_tempLinkList.Count - i),
                            _tempLinkList[i]));

                    if (NextPostPaginationArrow != null)
                    {
                        NextPostPaginationArrow.Click();
                        _tempLinkList.Clear();
                        new PostPage(_driver, _targetMedia).GetPostData();
                    }
                    else
                    {
                        _targetMedia.Complete();
                        Logger.Info("Finished capture of post data");
                    }
                }
            }
            catch (StaleElementReferenceException)
            {
                Logger.Error("Stale Element, Retrying");
                GetPostData();
            }
        }

        public void GetPostDataWithComments()
        {
            try
            {
                _webHelper.WaitForElement(By.CssSelector(".eo2As"), 2000);

                if (MultiSrcPostChevron != null)
                {
                    if (VideoSourceClass.Any())
                        foreach (var webElement in VideoSourceClass)
                            _tempLinkList.Add(webElement.GetAttribute("src"));
                    else if (ImageSourceClass.Any())
                        foreach (var webElement in ImageSourceClass)
                        {
                            var stringList = webElement.GetAttribute("srcset").Split(',');
                            var index = Array.FindIndex(stringList, row => row.Contains("1080w"));
                            _tempLinkList.Add(stringList[index].Remove(stringList[index].Length - 6));
                        }

                    MultiSrcPostChevron.Click();
                    GetPostDataWithComments();
                }
                else
                {
                    if (VideoSourceClass.Any())
                        foreach (var webElement in VideoSourceClass)
                            _tempLinkList.Add(webElement.GetAttribute("src"));
                    else if (ImageSourceClass.Any())
                        foreach (var webElement in ImageSourceClass)
                        {
                            var stringList = webElement.GetAttribute("srcset").Split(',');
                            var index = Array.FindIndex(stringList, row => row.Contains("1080w"));
                            _tempLinkList.Add(stringList[index].Remove(stringList[index].Length - 6));
                        }

                    _tempLinkList = _tempLinkList.Distinct().ToList();
                    var timeStamp = _webHelper.RefineTimeStamp();

                    for (var i = 0; i < _tempLinkList.Count; i++)
                        _targetMedia.Post(new KeyValuePair<string, string>(timeStamp + " " + (_tempLinkList.Count - i),
                            _tempLinkList[i]));

                    ViewAllCommentsClass?.Click();

                    var commentUserList =
                        CommentUserList.Select(username => username.GetAttribute("title")).ToList();
                    var commentTextList = CommentTextList.Select(text => text.Text).ToList();
                    
                    //First element is a header for the post with the name of the account that posted it
                    commentUserList.RemoveAt(0);

                    var zippedList = commentUserList
                        .Select((t, i) => new KeyValuePair<string, string>(t, commentTextList[i])).ToList();

                    _targetText.Post(new KeyValuePair<string, List<KeyValuePair<string, string>>>(timeStamp, zippedList));


                    if (NextPostPaginationArrow != null)
                    {
                        NextPostPaginationArrow.Click();
                        _tempLinkList.Clear();
                        new PostPage(_driver, _targetMedia, _targetText).GetPostDataWithComments();
                    }
                    else
                    {
                        _targetMedia.Complete();
                        _targetText.Complete();
                        Logger.Info("Finished capture of post data");
                    }
                }
            }
            catch (StaleElementReferenceException)
            {
                Logger.Error("Stale Element, Retrying");
                GetPostData();
            }
        }
    }
}