namespace PushNotification.WebSites.Web.Serializers
{
    using System.Collections.Specialized;
    using System.Web;
    using PushNotification.WebSites.Web.Infrastructure;

    public class FormatSerializerFactory : IFormatSerializerFactory
    {
        private IFormatSerializer jsonSerializer;
        private IFormatSerializer jsonpSerializer;
        private IFormatSerializer xmlSerializer;

        private IFormatSerializer JsonSerializer
        {
            get { return this.jsonSerializer ?? (this.jsonSerializer = new JsonSerializer()); }
        }

        private IFormatSerializer JsonpSerializer
        {
            get { return this.jsonpSerializer ?? (this.jsonpSerializer = new JsonSerializer()); }
        }

        private IFormatSerializer XmlSerializer
        {
            get { return this.xmlSerializer ?? (this.xmlSerializer = new XmlSerializer()); }
        }

        public IFormatSerializer GetSerializer(NameValueCollection headers, NameValueCollection queryString)
        {
            // Check content type too.
            var mimeType = headers["Accept"] ?? HttpConstants.MimeApplicationAtomXml;
            string callbackName = string.Empty;

            if (HttpContext.Current != null)
            {
                callbackName = queryString["jsonCallback"] ?? queryString["callback"];
            }

            if (mimeType.Contains(HttpConstants.MimeApplicationJson))
            {
                return string.IsNullOrWhiteSpace(callbackName) ? this.JsonSerializer : this.JsonpSerializer;
            }

            if (mimeType.Contains("text/xml") || mimeType.Contains("application/xml"))
            {
                return this.XmlSerializer;
            }

            if (!string.IsNullOrWhiteSpace(callbackName))
            {
                return this.JsonpSerializer;
            }

            return this.XmlSerializer;
        }
    }
}