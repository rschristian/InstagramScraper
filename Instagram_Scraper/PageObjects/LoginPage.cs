using Instagram_Scraper.Utility;
using OpenQA.Selenium;

namespace Instagram_Scraper.PageObjects
{
    public class LoginPage
    {
        private readonly IWebDriver _driver;

        private readonly WebDriverExtensions _webHelper;

        public LoginPage(IWebDriver driver)
        {
            _webHelper = new WebDriverExtensions(driver);
            _driver = driver;
        }

        private IWebElement UsernameField => _driver.FindElement(By.Name("username"));

        private IWebElement PasswordField => _driver.FindElement(By.Name("password"));

        private IWebElement LoginButton => _driver.FindElement(By.CssSelector(".L3NKy"));

        public void Login(string username, string password)
        {
            _driver.Navigate().GoToUrl("http://www.instagram.com/accounts/login");

            _webHelper.WaitForElement(By.CssSelector(".K-1uj"), 5000);

            UsernameField.SendKeys(username);
            PasswordField.SendKeys(password);

            LoginButton.Click();

            //.piCib is the alert that pops up when you first log in. The drivers don't save cookies,
            //so each run will be a "new" login.
            //COOzN is the sidebar, lower in the DOM. Hopefully the user is fully logged in by the time this is found.
            //TODO figure out what I'm doing here
            _webHelper.WaitForElement(By.CssSelector(".COOzN"), 2000);
        }
    }
}