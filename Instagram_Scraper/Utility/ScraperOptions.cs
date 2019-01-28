namespace Instagram_Scraper.Utility
{
    public class ScraperOptions
    {
        public readonly string TargetAccount;
        
        public readonly string Username;
        
        public readonly string Password;
        
        public readonly bool Headless;
        
        public readonly bool ScrapeStory;
        
        public readonly bool ScrapeComments;

        public readonly bool OnlyScrapeStory;
        
        public readonly bool FireFoxProfile;
        
        public readonly string FolderSavePath;
        
        public ScraperOptions(string targetAccount, string username, string password, bool headless, bool scrapeStory, 
            bool scrapeComments, bool onlyScrapeStory, bool firefoxProfile, string folderSavePath)
        {
            TargetAccount = targetAccount;
            Username = username;
            Password = password;
            Headless = headless;
            ScrapeStory = scrapeStory;
            ScrapeComments = scrapeComments;
            OnlyScrapeStory = onlyScrapeStory;
            FireFoxProfile = firefoxProfile;
            FolderSavePath = folderSavePath;
        }
    }
}