namespace PushNotification.WebSites.Web.CloudServices.Notifications
{
    using System;

    public class Endpoint
    {
        public virtual string ApplicationId { get; set; }

        public virtual string TileId { get; set; }

        public virtual string ClientId { get; set; }

        public string ChannelUri { get; set; }

        public string UserId { get; set; }

        public DateTime ExpirationTime { get; set; }

        public string DeviceType { get; set; }

        public static T To<T>(Endpoint endpoint) where T : Endpoint
        {
            if (endpoint.GetType() == typeof(T))
                return endpoint as T;

            var destination = Activator.CreateInstance<T>();

            destination.ApplicationId = endpoint.ApplicationId;
            destination.TileId = endpoint.TileId;
            destination.ClientId = endpoint.ClientId;
            destination.ChannelUri = endpoint.ChannelUri;
            destination.UserId = endpoint.UserId;
            destination.ExpirationTime = endpoint.ExpirationTime;
            destination.DeviceType = endpoint.DeviceType;

            return destination;
        }
    }
}
