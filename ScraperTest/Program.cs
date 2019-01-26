using System;
using Instagram_Scraper;
using Instagram_Scraper.Utility;

namespace ScraperTest
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var correctRunPercentage = 0;

            var totalScraperRunTime = 0.000;
            
            const int testPasses = 5;
            for (var i = 0; i < testPasses; i++)
            {
                using (var outputCapture = new OutputCapture())
                {
                    var watch = System.Diagnostics.Stopwatch.StartNew();
                    
                    WebScraper.SetUp(new ScraperOptions("***REMOVED***", false, string.Empty, string.Empty,
                        string.Empty, true, true, false));
                
                    var stuff = outputCapture.Captured.ToString();
                    if (stuff.Contains("24 Downloading:")) correctRunPercentage++;
                    
                    watch.Stop();
                    totalScraperRunTime = totalScraperRunTime + watch.ElapsedMilliseconds;
                }
            }
            Console.WriteLine("{0}% of runs returned all links", correctRunPercentage*(100/testPasses));
            
            Console.WriteLine("It took the scraper {0} seconds on average, per test", totalScraperRunTime/(testPasses*1000.00));
        }
    }
}