using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks.Dataflow;
using NLog;

namespace Instagram_Scraper.Utility
{
    public static class DownloadManager
    {
        private static readonly Logger Logger = LogManager.GetLogger("Download Manager");

        public static async void ConsumeMediaAsync(string path, ISourceBlock<KeyValuePair<string, string>> source)
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
                Logger.Info("ConsumeMediaAsync|" + filesProcessed + " Processing: " + fileName);
                var fileExists = Directory.GetFiles(path, fileName + ".*");
                if (fileExists.Length > 0)
                    continue;

                Logger.Info("ConsumeMediaAsync|" + filesProcessed + " Downloading: " + fileName);

                if (fileUri.Contains(".mp4"))
                    client.DownloadFileAsync(new Uri(fileUri), path + fileName + ".mp4");
                else
                    client.DownloadFileAsync(new Uri(fileUri), path + fileName + ".jpg");

                filesDownloaded++;
            }

            Logger.Info("ConsumeMediaAsync|" + "Processed {0} files.", filesProcessed);
            Logger.Info("ConsumeMediaAsync|" + "Downloaded {0} files.", filesDownloaded);
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
                Logger.Info("ConsumeTextAsync|" + textFilesProcessed + " Processing Text: " + postDate);
                if (File.Exists(filePath)) continue;

                Logger.Info("ConsumeTextAsync|" + textFilesProcessed + " Downloading Text: " + postDate);

                using (var sw = File.CreateText(filePath))
                {
                    foreach (var (user, commentText) in commentData) sw.WriteLine(user + ": " + commentText);
                }

                textFilesDownloaded++;
            }

            Logger.Info("ConsumeTextAsync|" + "Processed {0} text files.", textFilesProcessed);
            Logger.Info("ConsumeTextAsync|" + "Downloaded {0} text files.", textFilesDownloaded);
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
                
                Logger.Info("ConsumeStoryAsync|" + storyItemsProcessed + " Processing: " + storyName);
                
                if (storyUri.Contains(".mp4"))
                    client.DownloadFileAsync(new Uri(storyUri), dirPathTemp + storyName + ".mp4");
                else
                    client.DownloadFileAsync(new Uri(storyUri), dirPathTemp + storyName + ".jpg");
            }
            
            System.Threading.Thread.Sleep(2000);

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
                //Difference new -> existing, and is used as newList's index - offset, but in the existing
                var currentOffset = 0;

                void Test()
                {
                   for (var i = 0; i < newStoryList.Count; i++)
                    {
                        if (i + currentOffset < existingStoryList.Count)
                        {
                            // Logger.Debug("New " + newStoryList[i].Key + " equals Existing " + existingStoryList[i + currentOffset].Key + ": {0}", 
                            //     newStoryList[i].Value.SequenceEqual(existingStoryList[i + currentOffset].Value));
                            if (!newStoryList[i].Value.SequenceEqual(existingStoryList[i + currentOffset].Value) &&
                                i + 1 + currentOffset < existingStoryList.Count)
                            {
                                for (var j = i + 1; j < existingStoryList.Count; j++)
                                {
                                    // Logger.Debug("New " + newStoryList[i].Key + " equals Existing " + existingStoryList[j + currentOffset].Key + ": {0}", 
                                    //     newStoryList[i].Value.SequenceEqual(existingStoryList[j + currentOffset].Value));
    
                                    if (newStoryList[i].Value.SequenceEqual(existingStoryList[j + currentOffset].Value))
                                    {
                                        currentOffset += 1;
                                        break;
                                    }
                                    if (j + 1 < existingStoryList.Count) continue;
                                    
                                    Logger.Debug("Maxed out inner comparisons");
                                    MoveExistingFiles(dirPath, existingStoryList, newStoryList.Count - i);
                                    storyItemsDownloaded += MoveRemainingNewFiles(dirPathTemp, dirPath, newStoryList, i);
                                    return;
                                }
                            }
                            else if (!newStoryList[i].Value.SequenceEqual(existingStoryList[i + currentOffset].Value))
                            {
                                Logger.Debug("Hit second else branch");
                                MoveExistingFiles(dirPath, existingStoryList, newStoryList.Count - i);
                                storyItemsDownloaded += MoveRemainingNewFiles(dirPathTemp, dirPath, newStoryList, i);
                                return;
                            }
                        }
                        else
                        {
                            Logger.Debug("Hit first else branch");
                            MoveExistingFiles(dirPath, existingStoryList, newStoryList.Count - i);
                            storyItemsDownloaded += MoveRemainingNewFiles(dirPathTemp, dirPath, newStoryList, i);
                            return;
                        }
                    } 
                }
                Test();
            }
            
            //Cleans up temp folder
            Directory.Delete(dirPathTemp, true);
            
            Logger.Info("ConsumeStoryAsync|" + "Processed {0} story items.", storyItemsProcessed);
            Logger.Info("ConsumeStoryAsync|" + "Downloaded {0} story items.", storyItemsDownloaded);
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