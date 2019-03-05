using Gtk;
using Instagram_Scraper.UserInterface;

namespace Instagram_Scraper
{
    internal static class InstagramScraper
    {
        private static void Main()
        {
            Application.Init();
            new MainWindow().Show();
            Application.Run();
        }
    }
}