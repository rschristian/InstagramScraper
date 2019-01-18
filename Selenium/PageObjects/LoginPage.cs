using OpenQA.Selenium;
using Selenium.Utility;

namespace Selenium.PageObjects
{
    public class LoginPage
    {
        private readonly WebDriverExtensions _webHelper;

        private readonly IWebDriver _driver;

        public LoginPage(IWebDriver driver)
        {
            _webHelper = new WebDriverExtensions(driver);
            _driver = driver;
        }
        
        //TODO Change this over to safe-fail elements, so that, if null, display an error telling me to update the selectors
        private IWebElement UsernameField => _driver.FindElement(By.Name("username"));
        
        private IWebElement PasswordField => _driver.FindElement(By.Name("password"));
        
        private IWebElement LoginButton => _driver.FindElement(By.CssSelector(".L3NKy"));

        public void Login(string username, string password)
        {
            _driver.Navigate().GoToUrl("http://www.instagram.com/accounts/login");
            
            _webHelper.WaitForElement(By.CssSelector(".K-1uj"), 5);
            
            UsernameField.SendKeys(username);
            PasswordField.SendKeys(password);
            
            LoginButton.Click();
            
            //.piCib is the alert that pops up when you first log in. The drivers don't save cookies,
            //so each run will be a "new" login.
            //COOzN is the sidebar, lower in the DOM. Hopefully the user is fully logged in by the time this is found.
            _webHelper.WaitForElement(By.CssSelector(".COOzN"), 2);
        }
    }
}