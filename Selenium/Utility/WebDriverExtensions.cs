using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Selenium.Utility
{
    public class WebDriverExtensions
    {
        private readonly IWebDriver _driver;
        
        public WebDriverExtensions(IWebDriver driver)
        {
            _driver = driver;
        }
        
        public IWebElement FindElement(By by, int timeoutInSeconds)
        {
            if (timeoutInSeconds <= 0) return _driver.FindElement(@by);
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(timeoutInSeconds));
            return wait.Until(drv => drv.FindElement(@by));
        }
        
        public IWebElement SafeFindElement(string selector)
        {
            try
            {
                return _driver.FindElement(By.CssSelector(selector));
            }
            catch (NoSuchElementException)
            {
                return null;
            }
        }
        
        public IEnumerable<IWebElement> SafeFindElements(string selector)
        {
            try
            {
                return _driver.FindElements(By.CssSelector(selector));
            }
            catch (NoSuchElementException)
            {
                return null;
            }
        }
        
        public static bool IsElementPresent(IWebElement element)
        {
            try
            {

                return element.Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
            catch (NullReferenceException)
            {
                return false;
            }
        }
        
        public static bool IsElementsPresent(IEnumerable<IWebElement> elementList)
        {
            try
            {
                if (elementList.Any(item => item.Displayed))
                {
                    return true;
                }
            }
            catch (NoSuchElementException)
            {
                return false;
            }
            catch (NullReferenceException)
            {
                return false;
            }
            return false;
        }

        public bool IsElementVisible(IWebElement element)
        {
            return element.Displayed && element.Enabled;
        }
    }
}