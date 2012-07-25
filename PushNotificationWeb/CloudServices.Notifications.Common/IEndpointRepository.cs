namespace PushNotification.WebSites.Web.CloudServices.Notifications
{
    using System;
    using System.Collections.Generic;

    public interface IEndpointRepository
    {
        IEnumerable<Endpoint> All();

        IEnumerable<Endpoint> AllThat(Func<Endpoint, bool> filterExpression);

        Endpoint Find(Func<Endpoint, bool> filterExpression);

        Endpoint Find(string applicationId, string tileId, string clientId);

        void InsertOrUpdate(Endpoint endpoint);

        void Delete(string applicationId, string tileId, string clientId);
    }
}
