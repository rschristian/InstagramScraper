namespace Instagram_Scraper.Utility
{
    public class ScraperOptions
    {
        public readonly string TargetAccount;
        
        public readonly bool ScrapeStory;
        
        public readonly string Username;
        
        public readonly string Password;
        
        public readonly string FolderSavePath;
        
        public readonly bool Headless;
        
        public readonly bool FireFoxProfile;
        
        public readonly bool ScrapeComments;

        public ScraperOptions(string targetAccount, bool scrapeStory, string username, string password,
            string folderSavePath, bool headless, bool firefoxProfile, bool scrapeComments)
        {
            TargetAccount = targetAccount;
            ScrapeStory = scrapeStory;
            Username = username;
            Password = password;
            FolderSavePath = folderSavePath;
            Headless = headless;
            FireFoxProfile = firefoxProfile;
            ScrapeComments = scrapeComments;
        }
    }
}