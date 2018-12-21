using System;
using OpenQA.Selenium;
using Selenium.Utility;
using SeleniumExtras.PageObjects;

namespace Selenium.PageObjects
{
    public class PostPage
    {
        private readonly IWebDriver _driver;

        private int _contentInSet;

        public PostPage(IWebDriver driver)
        {
            _driver = driver;
            PageFactory.InitElements(driver, this);
        }
        
        [FindsBy(How = How.CssSelector, Using = "_97aPb")]
        private IWebElement _multiSrcPostChevronRoot;
        
        [FindsBy(How = How.CssSelector, Using = ".coreSpriteRightChevron")]
        private IWebElement _multiSrcPostChevron;
        
        [FindsBy(How = How.CssSelector, Using = ".coreSpriteRightPaginationArrow")]
        private IWebElement _nextPostPaginationArrow;
        
        [FindsBy(How = How.CssSelector, Using = ".kPFhm img")]
        private IWebElement _imageSourceClass;
        
        [FindsBy(How = How.CssSelector, Using = ".tWeCl")]
        private IWebElement _videoSourceClass;

        public void GetPostData()
        {
            _driver.FindElement(By.CssSelector(".kPFhm"), 1);
            Console.WriteLine(_driver.Url);
            if (IsElementPresent(_multiSrcPostChevron))
            {
                _contentInSet++;
                if (IsElementPresent(_videoSourceClass))
                {
                    Console.WriteLine(_videoSourceClass.GetAttribute("src"));
                }
                else if (IsElementPresent(_imageSourceClass))
                {
                    var links = _driver.FindElements(By.CssSelector(".kPFhm img"));
                    Console.WriteLine(links);
//                    var links = _imageSourceClass.GetAttribute("srcset").Split(',');
                    foreach (var link in links)
                    {
                        var x = link.GetAttribute("srcset").Split(',');
                        Console.WriteLine(x);
                        foreach (var y in x)
                        {
                            if (y.Contains("1080w"))
                            {
                                Console.WriteLine(y.Remove(y.Length-6));
                            } 
                        }
                    }
                }
                _multiSrcPostChevron.Click();
                GetPostData();
            }
            else
            {
                _contentInSet++;
                if (IsElementPresent(_videoSourceClass))
                {
                    Console.WriteLine(_videoSourceClass.GetAttribute("src"));
                }
                else if (IsElementPresent(_imageSourceClass))
                {
                    var links = _imageSourceClass.GetAttribute("srcset").Split(',');
                    foreach (var link in links)
                    {
                        if (!link.Contains("1080w")) continue;
                        Console.WriteLine(link.Remove(link.Length-6));
                    }
                }



                if (IsElementPresent(_nextPostPaginationArrow))
                {
                    _nextPostPaginationArrow.Click();
                    _contentInSet = 1;
                    GetPostData();
                }
                else
                {
                    Console.WriteLine("Finished");
                    //finish
                }
            }
            
        }
        
        private static bool IsElementPresent(IWebElement element)
        {
            try
            {
                return element.Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }
    }
}