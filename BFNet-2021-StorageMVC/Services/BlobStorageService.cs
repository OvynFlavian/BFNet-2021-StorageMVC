using Azure.Storage.Blobs;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BFNet_2021_StorageMVC.Services
{
    public class BlobStorageService
    {
        private string _AccessKey = string.Empty;

        public BlobStorageService(string AccessKey)
        {
            _AccessKey = AccessKey;
        }

        public string UploadFile(string fileName, byte[] fileData, string fileMimeType)
        {
            try
            {

                var _task = Task.Run(() => this.UploadFileAsync(fileName, fileData, fileMimeType));
                _task.Wait();
                string fileUrl = _task.Result;
                return fileUrl;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<string> ReadFiles()
        {
            try
            {
                var _task = Task.Run(() => this.ReadFilesAsync());
                _task.Wait();
                List<string> fileUrls = _task.Result;

                return fileUrls;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private string GenerateFileName(string fileName)
        {
            string str = string.Empty;
            string[] strName = fileName.Split(".");
            str = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd") + "/" + DateTime.Now.ToUniversalTime().ToString("yyyyMMdd\\THHmmssfff") + "." + strName[strName.Length - 1];

            return str;
        }

        private async Task<List<string>> ReadFilesAsync()
        {
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(_AccessKey);
            CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            string strContainerName = "images";
            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(strContainerName);

            List<string> uris = new List<string>();

            BlobContinuationToken continuationToken = null;
            do
            {
                var segment = await cloudBlobContainer.ListBlobsSegmentedAsync(string.Empty, true, BlobListingDetails.Metadata, 50, continuationToken, null, null);
                foreach (CloudBlob item in segment.Results)
                {
                    uris.Add(item.Uri.ToString());
                }
                continuationToken = segment.ContinuationToken;
            } while (continuationToken != null);

            return uris;
        }
        private async Task<string> UploadFileAsync(string fileName, byte[] fileData, string fileMimeType)
        {
            try
            {
                CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(_AccessKey);
                CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
                string strContainerName = "images";
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(strContainerName);
                string strFileName = this.GenerateFileName(fileName);

                if (await cloudBlobContainer.CreateIfNotExistsAsync())
                {
                    await cloudBlobContainer.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
                }

                if (strFileName != null && fileData != null)
                {
                    CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(strFileName);
                    cloudBlockBlob.Properties.ContentType = fileMimeType;
                    await cloudBlockBlob.UploadFromByteArrayAsync(fileData, 0, fileData.Length);
                    return cloudBlockBlob.Uri.AbsoluteUri;
                }
                return "";
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
