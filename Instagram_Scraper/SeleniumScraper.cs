using Gtk;
using Selenium.UserInterface;

namespace Selenium
{
    internal static class SeleniumScraper
    {
        private static void Main()
        {
            Application.Init();
            new MainWindow().Show();
            Application.Run();
        }
    }
}