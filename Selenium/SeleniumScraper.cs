using Gtk;
using Selenium.UserInterface;

namespace Selenium
{
    internal static class SeleniumScraper
    {
        private static void Main(string[] args)
        {
            Application.Init();
            var win = new MainWindow();
            win.Show();
            Application.Run();
        }
    }
}