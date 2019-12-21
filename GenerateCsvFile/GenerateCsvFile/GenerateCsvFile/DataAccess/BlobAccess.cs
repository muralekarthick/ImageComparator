using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace GenerateCsvFile.DataAccess
{
    public class BlobAccess
    {
        string _blobConnectionString;
        public BlobAccess(string connectionString)
        {
            _blobConnectionString = connectionString;
        }
        public CloudBlobContainer GetBlobContainer(string containerName)
        {
            try
            {
                TimeSpan backOffPeriod = TimeSpan.FromSeconds(5);
                var blobStorageAccount = CloudStorageAccount.Parse(_blobConnectionString);
                var blobClient = blobStorageAccount.CreateCloudBlobClient();

                BlobRequestOptions blobRequestOptions = new BlobRequestOptions()
                {
                    //TODO: Set Max File Size
                    SingleBlobUploadThresholdInBytes = (2 * 1024 * 1024), // Converting MB to bytes
                    ParallelOperationThreadCount = 4,
                    RetryPolicy = new ExponentialRetry(backOffPeriod, 3),
                };
                blobClient.DefaultRequestOptions = blobRequestOptions;
                return blobClient.GetContainerReference(containerName);
            }
            catch (Exception ex)
            {
                //TODO: Handle exception
                throw;
            }
        }


        public string GetFileAsUri(bool expiryRequired, int expiryDurationHours, bool readWritePermissionsRequired, string fileName, string containerName)
        {
            try
            {
                CloudBlobContainer container = GetBlobContainer(containerName);
                CloudBlob file = container.GetBlobReference(fileName);
                SharedAccessBlobPolicy sasConstraints = new SharedAccessBlobPolicy();
                sasConstraints.SharedAccessStartTime = DateTime.UtcNow.AddMinutes(-5);
                if (expiryRequired)
                    sasConstraints.SharedAccessExpiryTime = DateTime.UtcNow.AddHours(expiryDurationHours);
                else
                    sasConstraints.SharedAccessExpiryTime = DateTime.MaxValue;

                if (readWritePermissionsRequired)
                    sasConstraints.Permissions = SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.Write;
                else
                    sasConstraints.Permissions = SharedAccessBlobPermissions.Read;
                string sasBlobToken = file.GetSharedAccessSignature(sasConstraints);
                return file.Uri + sasBlobToken;
            }
            catch (Exception ex)
            {
                //TODO: Handle exception
                throw;
            }
        }


        public Task UploadBlob(Stream fileStream, string fileName, string containerName)
        {
            try
            {
                var container = GetBlobContainer(containerName);
                fileStream.Position = 0;
                var blockBlob = container.GetBlockBlobReference(fileName);
                return blockBlob.UploadFromStreamAsync(fileStream);
            }
            catch (Exception ex)
            {
                //TODO: Handle exception
                throw;
            }
        }
    }
}
