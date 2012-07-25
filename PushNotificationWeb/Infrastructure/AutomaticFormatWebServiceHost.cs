namespace PushNotification.WebSites.Web.Infrastructure
{
    using System;
    using System.ServiceModel.Description;
    using System.ServiceModel.Web;

    public class AutomaticFormatWebServiceHost : WebServiceHost
    {
        public AutomaticFormatWebServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }

        protected override void OnOpening()
        {
            base.OnOpening();

            foreach (var ep in this.Description.Endpoints)
            {
                var endpoint = ep.Behaviors.Find<WebHttpBehavior>();
                if (endpoint != null)
                {
                    endpoint.AutomaticFormatSelectionEnabled = true;
                }
            }
        }
    }
}