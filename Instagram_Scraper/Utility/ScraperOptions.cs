namespace Instagram_Scraper.Utility
{
    public class ScraperOptions
    {
        public string TargetAccount;
        
        public bool ScrapeStory;
        
        public string Username;
        
        public string Password;
        
        public string FolderSavePath;
        
        public bool Headless;
        
        public bool FireFoxProfile;
        
        public bool ScrapeComments;

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