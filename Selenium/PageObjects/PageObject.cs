using OpenQA.Selenium;
using SeleniumExtras.PageObjects;

namespace Selenium.PageObjects
{
    public class PageObject
    {
        protected PageObject(ISearchContext driver)
        {
            PageFactory.InitElements(driver, this);
        }
    }
}