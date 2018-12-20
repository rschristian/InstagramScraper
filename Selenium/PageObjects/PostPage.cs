using OpenQA.Selenium;
using SeleniumExtras.PageObjects;
using How = OpenQA.Selenium.Support.PageObjects.How;

namespace Selenium.PageObjects
{
    public class PostPage
    {
        private readonly IWebDriver _driver;

        public PostPage(IWebDriver driver)
        {
            _driver = driver;
            PageFactory.InitElements(driver, this);
        }
    }
}