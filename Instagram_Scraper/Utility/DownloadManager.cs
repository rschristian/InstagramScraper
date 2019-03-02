using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks.Dataflow;

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
            var dirPathTemp = dirPath + "Temp/";
            if (!File.Exists(dirPath)) Directory.CreateDirectory(dirPath);
            Directory.CreateDirectory(dirPathTemp);
            var storyItemsProcessed = 0; var storyItemsDownloaded = 0;

            while (await source.OutputAvailableAsync())
            {
                var client = new WebClient();
                var (storyName, storyUri) = source.Receive();
                storyItemsProcessed++;
                
                Console.WriteLine(storyItemsProcessed + " Processing: " + storyName);
                
                if (storyUri.Contains(".mp4"))
                    client.DownloadFileAsync(new Uri(storyUri), dirPathTemp + storyName + ".mp4");
                else
                    client.DownloadFileAsync(new Uri(storyUri), dirPathTemp + storyName + ".jpg");
            }
            
            System.Threading.Thread.Sleep(500);

            var existingStoryList = WebDriverExtensions.GetFilesFromDirectory(dirPath);
            var newStoryList = WebDriverExtensions.GetFilesFromDirectory(dirPathTemp);

            if (existingStoryList.Count == 0)
            {
                var fileInfo = new DirectoryInfo(dirPathTemp).GetFiles("*");
                foreach (var file in fileInfo)
                    File.Move(dirPathTemp + file.Name, dirPath + file.Name);
                storyItemsDownloaded += fileInfo.Length;
            }
            else
            {
                var watch = Stopwatch.StartNew();
                var maxIndexNewList = newStoryList.Count - 1;
                var maxIndexExistingList = existingStoryList.Count -1;
                var tempRunCounter = 0;
                var currentOffset = 0;
                var fileInfo = new DirectoryInfo(dirPathTemp).GetFiles("*");
                
                for (var i = maxIndexNewList; i > -1; i--)
                {
                    Console.WriteLine("Current offset: " + currentOffset);
                    Console.WriteLine("Current index: " + (maxIndexExistingList + currentOffset));
                    
                    Console.Write(newStoryList[i].Key + " equals " + existingStoryList[maxIndexExistingList + currentOffset + tempRunCounter].Key + " : ");
                    Console.WriteLine(newStoryList[i].Value.SequenceEqual(existingStoryList[maxIndexExistingList + currentOffset + tempRunCounter].Value));

                    if (newStoryList[i].Value.SequenceEqual(existingStoryList[maxIndexExistingList + currentOffset + tempRunCounter].Value))
                    {
                        tempRunCounter--;
                        continue;
                    }
                    
                    
                    for (var j = maxIndexExistingList + currentOffset -1; j > -1; j--)
                    {
                        Console.Write("Indent: " + newStoryList[i].Key + " equals " + existingStoryList[j].Key + " : ");
                        Console.WriteLine(newStoryList[i].Value.SequenceEqual(existingStoryList[j].Value));
                        
                        if (!newStoryList[i].Value.SequenceEqual(existingStoryList[j].Value) && j != 0) continue;
                        if (j != 0)
                        {
                            currentOffset--;
                            tempRunCounter--;
                            break;
                        }

                        existingStoryList.Reverse();
                        foreach (var (key, _) in existingStoryList)
                        {
                            char[] delimiterChars = { ' ', '.' };
                            var splits = key.Split(delimiterChars);
                            var newName = splits[0] + " " + splits[1] + " " + (int.Parse(splits[2]) + newStoryList.Count) + "." + splits[3];
                            File.Move(dirPath + key, dirPath + newName);
                        }
                        goto exitLoops;
                    }
                }
                exitLoops:
                Console.WriteLine("Offset at the end is: " + currentOffset);
                Console.WriteLine("Sort Time: " + watch.ElapsedMilliseconds / 1000.00 + " seconds");
            }
            
            
            //Cleans up temp folder
            Directory.Delete(dirPathTemp, true);
            
            Console.WriteLine("Processed {0} story items.", storyItemsProcessed);
            Console.WriteLine("Downloaded {0} story items.", storyItemsDownloaded);
        }
        
    }
}