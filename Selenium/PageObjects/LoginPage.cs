using System.Collections.Generic;
using System.Threading.Tasks.Dataflow;
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
        
        private IWebElement UsernameField => _driver.FindElement(By.Name("username"));
        
        private IWebElement PasswordField => _driver.FindElement(By.Name("password"));
        
        private IWebElement LoginButton => _driver.FindElement(By.CssSelector(".L3NKy"));

        public void Login(string username, string password)
        {
            _driver.Navigate().GoToUrl("http://www.instagram.com/accounts/login");
            
            _webHelper.FindElement(By.CssSelector(".K-1uj"), 5);
            
            UsernameField.SendKeys(username);
            PasswordField.SendKeys(password);
            
            LoginButton.Click();
            
            System.Threading.Thread.Sleep(500);
        }
    }
}