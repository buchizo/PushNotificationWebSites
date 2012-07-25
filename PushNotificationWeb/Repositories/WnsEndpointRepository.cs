namespace PushNotification.WebSites.Web.Repositories
{
    using System.Collections.Generic;
    using PushNotification.WebSites.Web.CloudServices.Notifications;

    public class WnsEndpointRepository : IWnsEndpointRepository
    {
        private readonly IEndpointRepository endpointRepository;

        public WnsEndpointRepository()
        {
            this.endpointRepository = NotificationServiceContext.Current.Configuration.StorageProvider;
        }

        public IEnumerable<Endpoint> GetAllEndpoints()
        {
            return this.endpointRepository.All();
        }

        public Endpoint GetEndpoint(string applicationId, string deviceId)
        {
            return this.endpointRepository.Find(e => e.ApplicationId == applicationId && e.DeviceId == deviceId);
        }
    }
}