namespace PushNotification.WebSites.Web.CloudServices.Notifications
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Web.Http;
    using System.Web.Routing;

    public static class RouteExtensions
    {
        public static void MapRegistrationServiceRoute(this RouteCollection routes, string prefix)
        {
            var handlers = NotificationServiceContext.Current.Configuration.DelegatingHandlers.ToArray();

            routes.MapRegistrationServiceRoute(prefix, handlers);
        }

        public static void MapRegistrationServiceRoute(this RouteCollection routes, string prefix, params DelegatingHandler[] handlers)
        {
            var currentConfiguration = GlobalConfiguration.Configuration;

            // Handlers
            currentConfiguration.AddDelegatingHandlers(handlers);
            
            // Routes
            routes.MapHttpRoute(
                name: prefix,
                routeTemplate: prefix + "/{applicationId}/{clientId}/{tileId}",
                defaults: new { Controller = "Endpoint", applicationId = RouteParameter.Optional, tileId = RouteParameter.Optional, clientId = RouteParameter.Optional });
        }
    }
}
