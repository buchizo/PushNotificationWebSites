namespace PushNotification.WebSites.Web.CloudServices.Notifications
{
    using System;

    public class NotificationServiceContext
    {
        private static readonly NotificationServiceContext Instance = new NotificationServiceContext();

        private readonly NotificationServiceConfig config = new NotificationServiceConfig();

        public static NotificationServiceContext Current
        {
            get { return Instance; }
        }

        public NotificationServiceConfig Configuration
        {
            get { return this.config; }
        }

        public void Configure(Action<NotificationServiceConfig> configure)
        {
            if (configure == null)
                throw new ArgumentException(Constants.ErrorParameterConfigureParamCannotBeNull, "configure");

            configure(this.config);
        }
    }
}
