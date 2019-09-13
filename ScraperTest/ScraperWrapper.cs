using System;
using Instagram_Scraper.Domain;
using Instagram_Scraper.Utility;
using NLog;

namespace ScraperTest
{
    internal static class Program
    {
        private static readonly Logger Logger = LogManager.GetLogger("Scraper Test");
        
        //Wrapper that can be used to run the program continuously, as a test. Not fully fleshed out, but it exists to
        //test consistency
        private static void Main()
        {
            var runsPassed = 0;

            var totalScraperRunTime = 0.000;
            
            const int testPasses = 5;
            for (var i = 0; i < testPasses; i++)
            {
                using (var outputCapture = new OutputCapture())
                {
                    var watch = System.Diagnostics.Stopwatch.StartNew();

                    try
                    {
                        InitializeScraper.SetUp(new ScraperOptions("", string.Empty, string.Empty,
                            true, false, false, false,string.Empty));
                    }
                    catch (NullReferenceException)
                    {
                        Logger.Error("Null reference, counting as failure.");
                        continue;
                    }
                    
                    
                    watch.Stop();
                    totalScraperRunTime += watch.ElapsedMilliseconds;
                
                    System.Threading.Thread.Sleep(2000);
                    var logMessages = outputCapture.Captured.ToString();
                    if (logMessages.Contains("Processed 93 files.")) runsPassed++;
                }
            }
            
            Console.WriteLine("{0}% of runs returned all links", runsPassed*(100/testPasses));
            
            Console.WriteLine("It took the scraper {0} seconds on average, per test", totalScraperRunTime/(testPasses*1000.00));
        }
    }
}
