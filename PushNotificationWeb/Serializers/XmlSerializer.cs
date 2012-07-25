namespace PushNotification.WebSites.Web.Serializers
{
    using System.Xml;
    using PushNotification.WebSites.Web.Infrastructure;

    public class XmlSerializer : IFormatSerializer
    {
        public string SerializeReply(object originalReply, out string contentType)
        {
            contentType = HttpConstants.MimeApplicationAtomXml;

            return (originalReply as XmlDocument).InnerXml;
        }
    }
}