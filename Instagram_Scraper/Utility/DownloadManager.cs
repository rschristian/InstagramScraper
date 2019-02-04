using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks.Dataflow;

namespace Instagram_Scraper.Utility
{
    public static class DownloadManager
    {
        public static async void ConsumeFilesAsync(string path, ISourceBlock<KeyValuePair<string, string>> source)
        {
            if (!File.Exists(path)) Directory.CreateDirectory(path);
            var filesProcessed = 0;
            var filesDownloaded = 0;

            while (await source.OutputAvailableAsync())
            {
                var client = new WebClient();
                var (key, value) = source.Receive();
                filesProcessed++;

                //Conflicted about whether or not this should exist. Doesn't save time, but can fix (or break)
                //previous downloads.
                Console.WriteLine(filesProcessed + " Processing: " + key);
                var fileExists = Directory.GetFiles(path, key + ".*");
                if (fileExists.Length > 0) continue;

                Console.WriteLine(filesProcessed + " Downloading: " + key);

                if (value.Contains(".mp4"))
                    client.DownloadFileAsync(new Uri(value), path + key + ".mp4");
                else
                    client.DownloadFileAsync(new Uri(value), path + key + ".jpg");

                filesDownloaded++;
            }

            Console.WriteLine("Processed {0} files.", filesProcessed);
            Console.WriteLine("Downloaded {0} files.", filesDownloaded);
        }

        public static async void ConsumeTextAsync(string path,
            ISourceBlock<KeyValuePair<string, List<KeyValuePair<string, string>>>> source)
        {
            var dirPath = path + "Text/";
            if (!File.Exists(dirPath)) Directory.CreateDirectory(dirPath);
            var textFilesProcessed = 0;
            var textFilesDownloaded = 0;

            while (await source.OutputAvailableAsync())
            {
                var (postDate, commentData) = source.Receive();
                var filePath = dirPath + postDate + ".txt";
                textFilesProcessed++;

                //TODO Comments may change, so there needs to be a way to update, potentially without removing old ones
                //TODO append profile text to the profile file
                Console.WriteLine(textFilesProcessed + " Processing Text: " + postDate);
                if (File.Exists(filePath)) continue;

                Console.WriteLine(textFilesProcessed + " Downloading Text: " + postDate);

                using (var sw = File.CreateText(filePath))
                {
                    foreach (var (user, commentText) in commentData) sw.WriteLine(user + ": " + commentText);
                }

                textFilesDownloaded++;
            }

            Console.WriteLine("Processed {0} text files.", textFilesProcessed);
            Console.WriteLine("Downloaded {0} text files.", textFilesDownloaded);
        }
    }
}