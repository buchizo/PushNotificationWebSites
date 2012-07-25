namespace PushNotification.WebSites.Web.CloudServices.Notifications.WindowsAzure
{
    using System;
    using System.Collections.Generic;
    using System.Data.Services.Client;
    using System.Linq;
    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.Samples.Common.Storage;

    public class WindowsAzureEndpointRepository : IEndpointRepository
    {
        protected readonly IAzureTable<EndpointTableRow> Table;

        private const string EndpointsTableName = "Endpoints";

        public WindowsAzureEndpointRepository(CloudStorageAccount storageAccount)
            : this(new AzureTable<EndpointTableRow>(storageAccount, EndpointsTableName))
        {
        }

        public WindowsAzureEndpointRepository(IAzureTable<EndpointTableRow> table)
        {
            this.Table = table;

            // Create the Push User Endpoints table if does not exist.
            this.Table.CreateIfNotExist();
        }

        private IQueryable<EndpointTableRow> Endpoints
        {
            get
            {
                return this.Table.Query;
            }
        }

        public IEnumerable<Endpoint> All()
        {
            return this.Endpoints.ToList().Select(Endpoint.To<Endpoint>).ToList();
        }

        public IEnumerable<Endpoint> AllThat(Func<Endpoint, bool> filterExpression)
        {
            if (filterExpression == null)
                throw new ArgumentNullException("filterExpression", "The filterExpression cannot be null.");

            try
            {
                return this.Endpoints.Where(filterExpression);
            }
            catch (DataServiceQueryException ex)
            {
                if (!ex.IsTablesNotFoundException())
                    throw;

                return null;
            }
        }

        public Endpoint Find(Func<Endpoint, bool> filterExpression)
        {
            return this.Endpoints.Where(filterExpression).ToList().FirstOrDefault();
        }

        public Endpoint Find(string applicationId, string tileId, string clientId)
        {
            var rowkey = EndpointTableRow.CreateRowKey(tileId, clientId);
            Endpoint endpoint = null;

            try
            {
                endpoint = this.Endpoints.Where(e => e.PartitionKey.Equals(applicationId) && e.RowKey.Equals(rowkey)).ToList().FirstOrDefault();
            }
            catch (DataServiceQueryException e)
            {
                if (e.Response.StatusCode != 404)
                {
                    throw;
                }
            }

            return endpoint;
        }

        public void InsertOrUpdate(Endpoint endpoint)
        {
            if (endpoint == null)
                throw new ArgumentNullException("endpoint", "Parameter endpoint cannot be null.");

            this.Table.AddOrUpdateEntity(Endpoint.To<EndpointTableRow>(endpoint));
        }

        public void Delete(string applicationId, string tileId, string clientId)
        {
            if (string.IsNullOrWhiteSpace(applicationId))
                throw new ArgumentNullException("applicationId");

            if (string.IsNullOrWhiteSpace(clientId))
                throw new ArgumentNullException("clientId");

            var storedEndpoint = this.Find(applicationId, tileId, clientId);

            if (storedEndpoint != null)
                this.Table.DeleteEntity(Endpoint.To<EndpointTableRow>(storedEndpoint));
        }
    }
}
