using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Selenium.Utility
{
    public static class DownloadManager
    {
        public static async Task<int> ConsumeAsync(string path, ISourceBlock<KeyValuePair<string, string>> source)
        {
            if(!File.Exists(path)) {Directory.CreateDirectory(path);}
            var filesProcessed = 0;
            
            while (await source.OutputAvailableAsync())
            {
                var client = new WebClient();
                var (key, value) = source.Receive();
        
                if (File.Exists(path + key + ".*")) continue;
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
            return filesProcessed;
        }
    }
}