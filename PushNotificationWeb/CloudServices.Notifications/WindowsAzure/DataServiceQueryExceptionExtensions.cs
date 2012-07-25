namespace PushNotification.WebSites.Web.CloudServices.Notifications.WindowsAzure
{
    using System.Data.Services.Client;

    public static class DataServiceQueryExceptionExtensions
    {
        public static bool IsTablesNotFoundException(this DataServiceQueryException ex)
        {
            return ex.InnerException != null
                && ex.InnerException is DataServiceClientException
                && ((DataServiceClientException)ex.InnerException).StatusCode == 404;
        }
    }
}
