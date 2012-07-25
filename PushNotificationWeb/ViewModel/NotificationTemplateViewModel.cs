namespace PushNotification.WebSites.Web.ViewModel
{
    using System.Collections.Generic;

    public class NotificationTemplateViewModel
    {
        public IEnumerable<BlobFileInfo> TileImages { get; set; }

        public IEnumerable<string> Priorities { get; set; }

        public IEnumerable<string> BadgeGlyphValueContent { get; set; }

        public IEnumerable<string> ToastAudioContent { get; set; }

        public string ChannelUrl { get; set; }

        public string ApplicationId { get; set; }

        public string DeviceId { get; set; }

        public string NotificationType { get; set; }

        public string NotificationTemplateType { get; set; }
    }
}