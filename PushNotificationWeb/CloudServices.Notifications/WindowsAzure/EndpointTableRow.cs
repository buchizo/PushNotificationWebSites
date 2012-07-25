namespace PushNotification.WebSites.Web.CloudServices.Notifications.WindowsAzure
{
    using System;
    using System.Data.Services.Common;
    using System.Globalization;
    using System.Text;
    using Microsoft.WindowsAzure.Samples.Common.Storage;

    [DataServiceEntity]
    [DataServiceKey(new[] { "PartitionKey", "RowKey" })]
    public class EndpointTableRow : Endpoint, ITableServiceEntity
    {
        private string rowKey;

        public EndpointTableRow()
        {
            this.rowKey = string.Empty;
        }

        public string PartitionKey
        {
            get { return this.ApplicationId; }
            set { this.ApplicationId = value; }
        }

        public string RowKey
        {
            get
            {
                if (string.IsNullOrEmpty(this.rowKey))
                    this.RowKey = CreateRowKey(this.TileId, this.ClientId);

                return this.rowKey;
            }

            set
            {
                this.rowKey = value;
            }
        }

        public virtual DateTime Timestamp { get; set; }

        public static string CreateRowKey(string tileId, string clientId)
        {
            var encodedTileId = Convert.ToBase64String(Encoding.UTF8.GetBytes(tileId ?? string.Empty)).Replace("/", "%");
            var encodedClientId = Convert.ToBase64String(Encoding.UTF8.GetBytes(clientId ?? string.Empty)).Replace("/", "%");

            return string.Format(CultureInfo.InvariantCulture, "{0}_{1}", encodedTileId, encodedClientId);
        }
    }
}
