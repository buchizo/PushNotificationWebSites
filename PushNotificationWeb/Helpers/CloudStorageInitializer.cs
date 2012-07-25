namespace PushNotification.WebSites.Web.Infrastructure.Helpers
{
    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.StorageClient;
    using PushNotification.WebSites.Web.Infrastructure;
    using PushNotification.WebSites.Web.Models;

    public class CloudStorageInitializer
    {
        public static void InitializeCloudStorage(CloudStorageAccount account)
        {
            CloudTableClient cloudTableClient = account.CreateCloudTableClient();
            CloudQueueClient cloudQueueClient = account.CreateCloudQueueClient();

            CreateUserPrivilegeTable(cloudTableClient);
        }

        private static void CreateUserPrivilegeTable(CloudTableClient cloudTableClient)
        {
            cloudTableClient.CreateTableIfNotExist(PrivilegesTableServiceContext.UserPrivilegeTableName);

            // Execute conditionally for development storage only.
            if (cloudTableClient.BaseUri.IsLoopback)
            {
                TableServiceContext context = cloudTableClient.GetDataServiceContext();
                var entity = new UserPrivilege { UserId = "UserId", Privilege = "Privilege" };

                context.AddObject(PrivilegesTableServiceContext.UserPrivilegeTableName, entity);
                context.SaveChangesWithRetries();
                context.DeleteObject(entity);
                context.SaveChangesWithRetries();
            }
        }
    }
}