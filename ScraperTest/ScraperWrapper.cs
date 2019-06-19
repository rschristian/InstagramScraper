using System;
using Instagram_Scraper;
using Instagram_Scraper.Domain;
using Instagram_Scraper.Utility;

namespace ScraperTest
{
    internal static class Program
    {
        //Wrapper that can be used to run the program continuously, as a test. Not fully fleshed out, but it exists to
        //test consistency
        private static void Main()
        {
            var correctRunPercentage = 0;

            var totalScraperRunTime = 0.000;
            
            const int testPasses = 5;
            for (var i = 0; i < testPasses; i++)
            {
                using (var outputCapture = new OutputCapture())
                {
                    var watch = System.Diagnostics.Stopwatch.StartNew();
                    
                    InitializeScraper.SetUp(new ScraperOptions("", string.Empty, string.Empty,
                        true, false, false, false,string.Empty));
                    
                    watch.Stop();
                    totalScraperRunTime = totalScraperRunTime + watch.ElapsedMilliseconds;
                
                    System.Threading.Thread.Sleep(2000);
                    var stuff = outputCapture.Captured.ToString();
                    if (stuff.Contains("Processed 42 files.")) correctRunPercentage++;
                }
            }
            
            Console.WriteLine("{0}% of runs returned all links", correctRunPercentage*(100/testPasses));
            
            Console.WriteLine("It took the scraper {0} seconds on average, per test", totalScraperRunTime/(testPasses*1000.00));
        }
    }
}
