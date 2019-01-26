using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks.Dataflow;

namespace Instagram_Scraper.Utility
{
    public static class DownloadManager
    {
        public static async void ConsumeAsync(string path, ISourceBlock<KeyValuePair<string, string>> source)
        {
            if(!File.Exists(path)) Directory.CreateDirectory(path);
            var filesProcessed = 0;
            var filesDownloaded = 0;
            
            while (await source.OutputAvailableAsync())
            {
                var client = new WebClient();
                var (key, value) = source.Receive();
                filesProcessed++;
        
                //Conflicted about whether or not this should exist. Doesn't save time, but can fix (or break)
                //previous downloads.
                var fileExists = Directory.GetFiles(path,key + ".*");
                if (fileExists.Length > 0) continue;
                
                Console.WriteLine((filesProcessed) + " Downloading: " + key);
                
                if (value.Contains(".mp4"))
                {
                    client.DownloadFileAsync(new Uri(value), path + key + ".mp4");
                }
                else
                {
                    client.DownloadFileAsync(new Uri(value), path + key + ".jpg");
                }
                
                filesProcessed++;
            }
            
            Console.WriteLine("Processed {0} files.", filesProcessed);
        }
    }
}