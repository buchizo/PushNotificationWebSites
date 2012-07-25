namespace PushNotification.WebSites.Web.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.ServiceModel.Activation;
    using System.Web.Mvc;
    using System.Web.Mvc.Html;
    using System.Web.Routing;
    using Microsoft.WindowsAzure.StorageClient;
    using PushNotification.WebSites.Web.ViewModel;

    public static class Extensions
    {
        private const string ErrorResponse = "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\" ?><error {0}><code>{1}</code><message xml:lang=\"en-US\">{2}</message></error>";
        private const string DataServiceNamespace = "xmlns=\"http://schemas.microsoft.com/ado/2007/08/dataservices/metadata\"";

        public static MvcHtmlString MenuItem(this HtmlHelper helper, string linkText, string actionName, string controllerName)
        {
            var li = new TagBuilder("li");
            var routeData = helper.ViewContext.RouteData;
            var currentAction = routeData.GetRequiredString("action");
            var currentController = routeData.GetRequiredString("controller");
            if (string.Equals(currentAction, actionName, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(currentController, controllerName, StringComparison.OrdinalIgnoreCase))
            {
                li.AddCssClass("selected");
            }

            li.InnerHtml = helper.ActionLink(linkText, actionName, controllerName).ToHtmlString();
            return MvcHtmlString.Create(li.ToString());
        }

        public static void AddWcfServiceRoute(this RouteCollection routes, Type dataServiceType, string prefix)
        {
            routes.Add(new ServiceRoute(prefix, new AutomaticFormatServiceHostFactory(), dataServiceType));
        }

        public static void AddWcfServiceRoute<TService>(this RouteCollection routes, string prefix)
        {
            AddWcfServiceRoute(routes, typeof(TService), prefix);
        }

        public static List<BlobFileInfo> GetAllBlobsInContainer(this CloudBlobClient blobClient, string container)
        {
            var blobContainer = blobClient.GetContainerReference(container);
			if (blobContainer.CreateIfNotExist())
			{
				var permissions = new BlobContainerPermissions();
				permissions.PublicAccess = BlobContainerPublicAccessType.Container;
				blobContainer.SetPermissions(permissions);
			}
			var allBlobs = blobContainer.ListBlobs(new BlobRequestOptions() { BlobListingDetails = BlobListingDetails.Metadata });

            var tileImages = new List<BlobFileInfo>();

            foreach (var blob in allBlobs)
            {
                var tileName = Path.GetFileName(blob.Uri.LocalPath);
                tileImages.Add(new BlobFileInfo { FileName = tileName, FileUri = blob.Uri.ToString() });
            }

            return tileImages;
        }
    }
}