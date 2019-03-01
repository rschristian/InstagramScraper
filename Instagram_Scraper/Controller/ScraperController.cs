using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks.Dataflow;
using Instagram_Scraper.Domain;
using Instagram_Scraper.PageObjects;
using OpenQA.Selenium;

namespace Instagram_Scraper.Controller
{
    public class ScraperController
    {
        private readonly IWebDriver _driver;

        public ScraperController(IWebDriver driver)
        {
            _driver = driver;
        }

        public void ExecuteScraper(ScraperOptions scraperOptions,
            ITargetBlock<KeyValuePair<string, string>> targetMedia)
        {
            var watch = Stopwatch.StartNew();
            if (scraperOptions.OnlyScrapeStory)
            {
                OnlyScrapeStory(scraperOptions, targetMedia);
                Console.WriteLine("Total Program Time: " + watch.ElapsedMilliseconds / 1000.00 + " seconds");
                return;
            }

            //Login
            if (!scraperOptions.Username.Equals(string.Empty))
                LoginToAccount(scraperOptions);

            //Profile Page
            var profilePage = new ProfilePage(_driver, scraperOptions.TargetAccount);
            profilePage.GetProfilePicture(targetMedia);

            //Story Page
            if (scraperOptions.ScrapeStory)
                ScrapeStory(profilePage, targetMedia);

            var postPage = profilePage.EnterPosts(targetMedia);


            //PostPage
            postPage.GetPostData();

            Console.WriteLine("Total Program Time: " + watch.ElapsedMilliseconds / 1000.00 + " seconds");
        }

        public void ExecuteScraper(ScraperOptions scraperOptions,
            ITargetBlock<KeyValuePair<string, string>> targetMedia,
            ITargetBlock<KeyValuePair<string, List<KeyValuePair<string, string>>>> targetText)
        {
            //Login
            var watch = Stopwatch.StartNew();
            if (!scraperOptions.Username.Equals(string.Empty))
                LoginToAccount(scraperOptions);

            //Profile Page
            var profilePage = new ProfilePage(_driver, scraperOptions.TargetAccount);
            profilePage.GetProfilePicture(targetMedia);
            profilePage.GetProfileText(targetText);

            //Story Page
            if (scraperOptions.ScrapeStory)
                ScrapeStory(profilePage, targetMedia);

            var postPage = profilePage.EnterPosts(targetMedia, targetText);


            //PostPage
            postPage.GetPostDataWithComments();

            Console.WriteLine("Total Program Time: " + watch.ElapsedMilliseconds / 1000.00 + " seconds");
        }

        private void LoginToAccount(ScraperOptions scraperOptions)
        {
            new LoginPage(_driver).Login(scraperOptions.Username, scraperOptions.Password);
        }

        private static void ScrapeStory(ProfilePage profilePage, ITargetBlock<KeyValuePair<string, string>> targetMedia)
        {
            var storyPage = profilePage.EnterStory(targetMedia);
            storyPage?.SaveStoryContent();
        }

        private void OnlyScrapeStory(ScraperOptions scraperOptions,
            ITargetBlock<KeyValuePair<string, string>> targetMedia)
        {
            LoginToAccount(scraperOptions);
            var storyPage = new ProfilePage(_driver, scraperOptions.TargetAccount).EnterStory(targetMedia);
            storyPage?.SaveStoryContent();
            targetMedia.Complete();
        }
    }
}