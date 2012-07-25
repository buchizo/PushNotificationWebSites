namespace PushNotification.WebSites.Web.Infrastructure
{
    using System.Collections.Generic;
    using PushNotification.WebSites.Web.ViewModel;

    public static class NotifcationSoundEvents
    {
        public static List<BlobFileInfo> GetSoundEvents()
        {
            var result = new List<BlobFileInfo>();
            result.Add(new BlobFileInfo() { FileName = "Default Notification", FileUri = "ms-winsoundevent:Notification.Default" });
            result.Add(new BlobFileInfo() { FileName = "Mail Notification", FileUri = "ms-winsoundevent:Notification.Mail" });
            result.Add(new BlobFileInfo() { FileName = "SMS Notification", FileUri = "ms-winsoundevent:Notification.SMS" });
            result.Add(new BlobFileInfo() { FileName = "IM Notification", FileUri = "ms-winsoundevent:Notification.IM" });
            result.Add(new BlobFileInfo() { FileName = "Reminder Notification", FileUri = "ms-winsoundevent:Notification.Reminder" });
            return result;
        }
    }
}