using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Threading.Tasks;

namespace AzureBlobBenchmark
{
    class Program
    {
        private const string accountName = "accountname";
        private const string accountKey = "accountkey";
        private const string containerName = "containername";
        private const string blobName = "blobname";
        private const int runCount = 50;

        static void Main(string[] args)
        {
            StorageCredentials cred = new StorageCredentials(accountName, accountKey);
            Microsoft.WindowsAzure.Storage.CloudStorageAccount acc = new CloudStorageAccount(cred, true);
            
            var client = acc.CreateCloudBlobClient();
            var container = client.GetContainerReference(containerName);
            
            TestMethod(container).Wait();
            TestMethod(container).Wait();
            TestMethod(container).Wait();
            
            var measurements = new List<TimeSpan>();

            for(int i = 0; i < runCount; i++)
            {
                TestMethod(container, measurements).Wait();
            }
            
            foreach(var m in measurements)
            {
                System.Console.WriteLine("Duration: {0}ms", m.TotalMilliseconds);
            }
            
            System.Console.WriteLine("Average duration: {0}ms", measurements.Sum(m => m.TotalMilliseconds) / runCount);
        }
        
        static async Task TestMethod(CloudBlobContainer container, List<TimeSpan> measurements = null)
        {
            using(var ms = new MemoryStream())
            {
                var sw = Stopwatch.StartNew();
                var blobRef = container.GetBlobReference(blobName);
                await blobRef.DownloadToStreamAsync(ms);
                sw.Stop();
                
                if(measurements != null)
                {
                    measurements.Add(sw.Elapsed);
                }
            }
        }
    }
}