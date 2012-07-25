namespace PushNotification.WebSites.Web.Serializers
{
    using System.Collections.Specialized;

    public interface IFormatSerializerFactory
    {
        IFormatSerializer GetSerializer(NameValueCollection headers, NameValueCollection queryString);
    }
}
