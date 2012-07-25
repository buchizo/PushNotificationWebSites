namespace PushNotification.WebSites.Web.Repositories
{
    using System;
    using System.Collections.Generic;
    using PushNotification.WebSites.Web.CloudServices.Notifications;

    [CLSCompliant(false)]
    public interface IWnsEndpointRepository
    {
        Endpoint GetEndpoint(string applicationId, string deviceId);

        IEnumerable<Endpoint> GetAllEndpoints();
    }
}