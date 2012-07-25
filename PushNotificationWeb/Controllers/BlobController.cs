namespace PushNotification.WebSites.Web.Controllers
{
    using System;
    using System.Collections.Specialized;
    using System.IO;
    using System.Web.Mvc;
    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.StorageClient;
    using PushNotification.WebSites.Web.Infrastructure;
    using PushNotification.WebSites.Web.Infrastructure.Helpers;

    [CustomAuthorize(Roles = PrivilegeConstants.AdminPrivilege)]
    public class BlobController : Controller
    {
        private readonly CloudBlobClient blobClient;

        public BlobController()
            : this(null)
        {
        }

        public BlobController(CloudBlobClient blobClient)
        {
            CloudStorageAccount account = null;

            if ((account = CloudStorageAccount.FromConfigurationSetting("DataConnectionString")) == null)
            {
                if (this.blobClient == null)
                {
                    throw new ArgumentNullException("cloudQueueClient", "Cloud Queue Client cannot be null if no configuration is loaded.");
                }
            }

            this.blobClient = blobClient ?? account.CreateCloudBlobClient();
        }

        public ActionResult Index()
        {
            return View(this.blobClient.GetAllBlobsInContainer(ConfigReader.GetConfigValue("TileImagesContainer")));
        }

        [HttpPost]
        public ActionResult AddBlob()
        {
            foreach (string inputName in Request.Files)
            {
                var file = Request.Files[inputName];

                if (file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    this.SaveImage(fileName, this.GetWNSContentType(file.ContentType), file.InputStream);
                }
            }

            return RedirectToAction("Index");
        }

        public ActionResult Delete(string url)
        {
            var blobContainer = this.blobClient.GetContainerReference(ConfigReader.GetConfigValue("TileImagesContainer"));
            var blob = blobContainer.GetBlobReference(url);
            blob.DeleteIfExists();

            return RedirectToAction("Index");
        }

        public string GetWNSContentType(string contentType)
        {
            // Content Types image/x-png and image/pjpeg are not supported
            string result = contentType;

            if (contentType.Contains("x-png"))
            {
                result = "image/png";
            }

            if (contentType.Contains("pjpeg"))
            {
                result = "image/jpeg";
            }

            return result;
        }

        private void SaveImage(string fileName, string contentType, Stream fileStream)
        {
            // Create a blob in container and upload image bytes to it
            var blobContainer = this.blobClient.GetContainerReference(ConfigReader.GetConfigValue("TileImagesContainer"));
            var blob = blobContainer.GetBlobReference(fileName);

            // Ensure you set the contentType for Tile notifications 
            blob.Properties.ContentType = contentType;

            // Create some metadata for this image
            var metadata = new NameValueCollection();
            metadata["DateTime"] = DateTime.UtcNow.ToString("yyyyMMddSS");
            metadata["Filename"] = fileName;

            // Add and commit metadata to blob
            blob.Metadata.Add(metadata);
            blob.UploadFromStream(fileStream);
        }
    }
}
