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
                var maxIndexExistingList = existingStoryList.Count -1;
                
                //TODO If the last item in existing can not be found in new, append all.
                //TODO If it can, start counting up the back end of existing to find when new no longer can be found
                
                //Difference new -> existing, and is used as newList's index - offset, but in the existing
                var currentOffset = 0;

                for (var i = 0; i < newStoryList.Count; i++)
                {
                    if (maxIndexExistingList - i + currentOffset < 0)
                    {
                        MoveExistingFiles(dirPath, existingStoryList, newStoryList.Count - i);
                        storyItemsDownloaded += MoveRemainingNewFiles(dirPathTemp, dirPath, newStoryList, i);
                        break;
                    }

                    if (newStoryList[i].Value.SequenceEqual(existingStoryList[i + currentOffset].Value)) continue;
                    for (var j = i + 1; j < existingStoryList.Count; j++)
                    {
                        if (newStoryList[i].Value.SequenceEqual(existingStoryList[j + currentOffset].Value))
                        {
                            currentOffset = -1;
                            break;
                        }

                        if (j != maxIndexExistingList) continue;
                        MoveExistingFiles(dirPath, existingStoryList, newStoryList.Count - i);
                    }

                    storyItemsDownloaded += MoveRemainingNewFiles(dirPathTemp, dirPath, newStoryList, i);
                    break;
                }
            }
            
            //Cleans up temp folder
            Directory.Delete(dirPathTemp, true);
            
            Console.WriteLine("Processed {0} story items.", storyItemsProcessed);
            Console.WriteLine("Downloaded {0} story items.", storyItemsDownloaded);
        }

        private static void MoveExistingFiles(string dirPath,
            IEnumerable<KeyValuePair<string, byte[]>> existingStoryList,
            int delta)
        {
            foreach (var (key, _) in existingStoryList)
            {
                char[] delimiterChars = { ' ', '.' };
                var splits = key.Split(delimiterChars);
                var newName = splits[0] + " story " + (int.Parse(splits[2]) + delta) + "." + splits[3];
                File.Move(dirPath + key, dirPath + newName);
            } 
        }
        
        private static int MoveRemainingNewFiles(string tempPath, string newPath,
            IReadOnlyList<KeyValuePair<string, byte[]>> newStoryList, int startingIndex)
        {
            var filesDownloaded = 0;
            for (var k = startingIndex; k < newStoryList.Count; k++)
            {
                File.Move(tempPath + newStoryList[k].Key, newPath + newStoryList[k].Key);
                filesDownloaded++;
            }

            return filesDownloaded;
        }
    }
}