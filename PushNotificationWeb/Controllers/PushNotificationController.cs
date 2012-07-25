namespace PushNotification.WebSites.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.ServiceRuntime;
    using Microsoft.WindowsAzure.StorageClient;
    using NotificationsExtensions;
    using NotificationsExtensions.BadgeContent;
    using NotificationsExtensions.ToastContent;
    using PushNotification.WebSites.Web.Infrastructure;
    using PushNotification.WebSites.Web.Infrastructure.Helpers;
    using PushNotification.WebSites.Web.Models;
    using PushNotification.WebSites.Web.Repositories;
    using PushNotification.WebSites.Web.ViewModel;

    [CustomAuthorize(Roles = PrivilegeConstants.AdminPrivilege)]
    public class PushNotificationController : Controller
    {
        private readonly IWnsEndpointRepository endpointsRepository;
        private readonly CloudQueueClient cloudQueueClient;
        private readonly CloudBlobClient blobClient;

        private readonly IAccessTokenProvider tokenProvider;

        public PushNotificationController()
            : this(
                    null,
                    null,
                    new WnsEndpointRepository(),
					new WnsAccessTokenProvider(ConfigReader.GetConfigValue("WNSPackageSID"), ConfigReader.GetConfigValue("WNSClientSecret")))
        {
        }

        [CLSCompliant(false)]
        public PushNotificationController(CloudQueueClient cloudQueueClient, CloudBlobClient cloudBlobClient, IWnsEndpointRepository endpointsRepository, IAccessTokenProvider tokenProvider)
        {
            this.endpointsRepository = endpointsRepository;

            CloudStorageAccount account = null;

			if ((account = CloudStorageAccount.Parse(ConfigReader.GetConfigValue("DataConnectionString"))) == null)
            {
                if (cloudQueueClient == null)
                {
                    throw new ArgumentNullException("cloudQueueClient", "Cloud Queue Client cannot be null if no configuration is loaded.");
                }
            }

            this.cloudQueueClient = cloudQueueClient ?? account.CreateCloudQueueClient();
            this.blobClient = cloudBlobClient ?? account.CreateCloudBlobClient();

            this.tokenProvider = tokenProvider;
        }

        public ActionResult Index()
        {
            return View(this.endpointsRepository.GetAllEndpoints());
        }

        [HttpPost]
        public ActionResult SendNotification(
            [ModelBinder(typeof(NotificationTemplateModelBinder))] INotificationContent notification,
            string channelUrl,
            NotificationPriority priority = NotificationPriority.Normal)
        {
            var options = new NotificationSendOptions()
                {
                    Priority = priority
                };

            NotificationSendResult result = notification.Send(new Uri(channelUrl), this.tokenProvider, options);

            object response = new
            {
                DeviceConnectionStatus = result.DeviceConnectionStatus.ToString(),
                NotificationStatus = result.NotificationStatus.ToString(),
                Status = result.LookupHttpStatusCode()
            };

            return this.Json(response);
        }

        public ActionResult GetNotificationTypes(string notificationType)
        {
            dynamic ddlTypes = null;

            switch (notificationType)
            {
                case "Badge":
                    ddlTypes = (new List<string> { "BadgeNumeric", "BadgeGlyph" }).Select(x => new { Id = x, Name = x });
                    break;
                case "Raw":
                    ddlTypes = (new List<string> { "Raw" }).Select(x => new { Id = x, Name = x });
                    break;
                case "Toast":
                    ddlTypes = Enum.GetNames(typeof(ToastType)).Select(x => new { Id = x, Name = x });
                    break;
                case "Tile":
                    ddlTypes = Enum.GetNames(typeof(TileType)).Select(x => new { Id = x, Name = x });
                    break;
            }

            return Json(ddlTypes, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetSendTemplate(NotificationTemplateViewModel templateOptions)
        {
            PartialViewResult result = null;

            switch (templateOptions.NotificationType)
            {
                case "Badge":
                    templateOptions.BadgeGlyphValueContent = Enum.GetNames(typeof(GlyphValue));
                    ViewBag.ViewData = templateOptions;
                    result = PartialView("_" + templateOptions.NotificationTemplateType);
                    break;
                case "Raw":
                    ViewBag.ViewData = templateOptions;
                    result = PartialView("_Raw");
                    break;
                case "Toast":
                    templateOptions.TileImages = this.blobClient.GetAllBlobsInContainer(ConfigReader.GetConfigValue("TileImagesContainer")).OrderBy(i => i.FileName).ToList();
                    templateOptions.ToastAudioContent = Enum.GetNames(typeof(ToastAudioContent));
                    templateOptions.Priorities = Enum.GetNames(typeof(NotificationPriority));
                    ViewBag.ViewData = templateOptions;
                    result = PartialView("_" + templateOptions.NotificationTemplateType);
                    break;
                case "Tile":
                    templateOptions.TileImages = this.blobClient.GetAllBlobsInContainer(ConfigReader.GetConfigValue("TileImagesContainer")).OrderBy(i => i.FileName).ToList();
                    ViewBag.ViewData = templateOptions;
                    result = PartialView("_" + templateOptions.NotificationTemplateType);
                    break;
            }

            return result;
        }

        private void Escape(List<string> items)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (!string.IsNullOrWhiteSpace(items[i]))
                {
                    items[i] = System.Security.SecurityElement.Escape(items[i]);
                }
            }
        }
    }
}
