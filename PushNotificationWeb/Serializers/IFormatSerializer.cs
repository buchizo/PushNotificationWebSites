namespace PushNotification.WebSites.Web.Serializers
{
    public interface IFormatSerializer
    {
        string SerializeReply(object originalReply, out string contentType);
    }
}
