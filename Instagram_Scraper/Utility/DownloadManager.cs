using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks.Dataflow;
using GLib;
using OpenQA.Selenium;
using DateTime = System.DateTime;

namespace Instagram_Scraper.Utility
{
    public static class DownloadManager
    {
        public static async void ConsumeFilesAsync(string path, ISourceBlock<KeyValuePair<string, string>> source)
        {
            if (!File.Exists(path)) Directory.CreateDirectory(path);
            var filesProcessed = 0; var filesDownloaded = 0;

            while (await source.OutputAvailableAsync())
            {
                var client = new WebClient();
                var (fileName, fileUri) = source.Receive();
                filesProcessed++;

                //Conflicted about whether or not this should exist. Doesn't save time, but can fix (or break)
                //previous downloads.
                Console.WriteLine(filesProcessed + " Processing: " + fileName);
                var fileExists = Directory.GetFiles(path, fileName + ".*");
                if (fileExists.Length > 0)
                    continue;

                Console.WriteLine(filesProcessed + " Downloading: " + fileName);

                if (fileUri.Contains(".mp4"))
                    client.DownloadFileAsync(new Uri(fileUri), path + fileName + ".mp4");
                else
                    client.DownloadFileAsync(new Uri(fileUri), path + fileName + ".jpg");

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
            var textFilesProcessed = 0; var textFilesDownloaded = 0;

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

        public static async void ConsumeStoryAsync(string path, ISourceBlock<KeyValuePair<string, string>> source)
        {
            var dirPath = path + "Stories/";
            if (!File.Exists(dirPath)) Directory.CreateDirectory(dirPath);
            Directory.CreateDirectory(dirPath + "Temp/");
            var storyItemsProcessed = 0; var storyItemsDownloaded = 0;
            
            
            var storyList = new List<KeyValuePair<string, string>>();
            var existingStoryList = WebDriverExtensions.GetFilesFromDirectory(dirPath);

            while (await source.OutputAvailableAsync())
            {
                var (storyName, storyUri) = source.Receive();
                storyItemsProcessed++;
                storyList.Add(new KeyValuePair<string, string>(storyName, storyUri));
            }

            Console.WriteLine("Story List count: " + storyList.Count);
            foreach (var (fileName, fileUri) in storyList)
            {
                var client = new WebClient();
                if (fileUri.Contains(".mp4"))
                    client.DownloadFileAsync(new Uri(fileUri), dirPath + "Temp/" + fileName + ".mp4");
                else
                    client.DownloadFileAsync(new Uri(fileUri), dirPath + "Temp/" + fileName + ".jpg");
            }
            
            System.Threading.Thread.Sleep(500);

            var newStoryList = WebDriverExtensions.GetFilesFromDirectory(dirPath + "Temp/");


            for (var i = 0; i < newStoryList.Count; i++)
            {
                if (i < existingStoryList.Count)
                {
                    Console.Write(newStoryList[i].Key + " equals " + existingStoryList[i].Key + " : ");
                    Console.WriteLine(newStoryList[i].Value.SequenceEqual(existingStoryList[i].Value));
                }
                else
                {
                    Console.WriteLine("More new story items than existing ones");
                }
            }
            
            //Cleans up temp folder
            Directory.Delete(dirPath + "Temp/", true);
            
            Console.WriteLine("Processed {0} story items.", storyItemsProcessed);
            Console.WriteLine("Downloaded {0} story items.", storyItemsDownloaded);
        }
        
    }
}