using System;
using GLib;
using Instagram_Scraper.UserInterface;
using Application = Gtk.Application;

namespace Instagram_Scraper
{
    internal static class InstagramScraper
    {
        private static void Main()
        {
            Application.Init();
            var h = new UnhandledExceptionHandler (OnException);
            ExceptionManager.UnhandledException += h;

            new MainWindow().Show();
            Application.Run();

            void OnException(UnhandledExceptionArgs args)
            {
                Console.WriteLine("Error in application: " + args.ExceptionObject);
                args.ExitApplication = false;
            }
        }
    }
}