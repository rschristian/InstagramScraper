using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks.Dataflow;
using Instagram_Scraper.Domain;
using Instagram_Scraper.Domain.PageObjects;
using NLog;
using OpenQA.Selenium;

namespace Instagram_Scraper.Controller
{
    public class ScraperController
    {
        
        private static readonly Logger Logger = LogManager.GetLogger("Scraper Controller");
        
        private readonly IWebDriver _driver;

        private readonly ScraperOptions _scraperOptions;

        private readonly ITargetBlock<KeyValuePair<string, string>> _targetMedia;

        private readonly ITargetBlock<KeyValuePair<string, List<KeyValuePair<string, string>>>> _targetText;

        private readonly ITargetBlock<KeyValuePair<string, string>> _targetStory;
        
        public ScraperController(IWebDriver driver, ScraperOptions scraperOptions,
            ITargetBlock<KeyValuePair<string, string>> targetMedia = null,
            ITargetBlock<KeyValuePair<string, List<KeyValuePair<string, string>>>> targetText = null,
            ITargetBlock<KeyValuePair<string, string>> targetStory = null)
        {
            _driver = driver;
            _scraperOptions = scraperOptions;
            _targetMedia = targetMedia;
            _targetText = targetText;
            _targetStory = targetStory;
        }

        public void ExecuteScraper()
        {
            var watch = Stopwatch.StartNew();
            if (_scraperOptions.OnlyScrapeStory)
            {
                OnlyScrapeStory();
                Logger.Info("Total Program Time: " + watch.ElapsedMilliseconds / 1000.00 + " seconds");
                return;
            }

            //Login
            if (!_scraperOptions.Username.Equals(string.Empty))
            {
                LoginToAccount();
            }

            //Profile Page
            var profilePage = new ProfilePage(_driver, _scraperOptions.TargetAccount);
            profilePage.GetProfilePicture(_targetMedia);
            if (_targetText != null)
            {
                profilePage.GetProfileText(_targetText);
            }

            //Story Page
            if (_scraperOptions.ScrapeStory)
            {
                ScrapeStory(profilePage);
            }

            //Posts Page
            var postPage = _targetText == null
                ? profilePage.EnterPosts(_targetMedia)
                : profilePage.EnterPosts(_targetMedia, _targetText);
            postPage.GetPostData();

            Logger.Info("Total Program Time: " + watch.ElapsedMilliseconds / 1000.00 + " seconds");
        }

        private void LoginToAccount()
        {
            new LoginPage(_driver).Login(_scraperOptions.Username, _scraperOptions.Password);
        }

        private void ScrapeStory(ProfilePage profilePage)
        {
            var storyPage = profilePage.EnterStory(_targetStory);
            storyPage?.SaveStoryContent();
        }

        public void OnlyScrapeStory()
        {
            LoginToAccount();
            var storyPage = new ProfilePage(_driver, _scraperOptions.TargetAccount).EnterStory(_targetStory);
            storyPage?.SaveStoryContent();
            _targetStory.Complete();
        }
    }
}