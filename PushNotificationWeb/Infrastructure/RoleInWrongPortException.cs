namespace PushNotification.WebSites.Web.Infrastructure
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class RoleInWrongPortException : Exception
    {
        public RoleInWrongPortException()
            : base("The Web Role is running in a wrong port. Please review the troubleshooting section of the Windows Azure Toolkit for Windows Phone for guidance in this issue.")
        {
        }

        public RoleInWrongPortException(string message)
            : base(message)
        {
        }

        public RoleInWrongPortException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected RoleInWrongPortException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}