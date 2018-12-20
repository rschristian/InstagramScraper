using Gtk;
using Selenium.UserInterface;

namespace Selenium
{
    class SeleniumScraper
    {
        private static void Main(string[] args)
        {
            Application.Init();
            new MainWindow().Show();
            Application.Run();
        }
    }
}