namespace PushNotification.WebSites.Web.CloudServices.Notifications
{
    using System.Linq;
    using System.Net.Http;
    using System.Web.Http;

    internal static class ConfigurationExtensions
    {
        internal static HttpConfiguration AddDelegatingHandlers(this HttpConfiguration config, params DelegatingHandler[] handlers)
        {
            handlers.ToList().ForEach(t => config.MessageHandlers.Add(t));

            return config;
        }
    }
}
