namespace PushNotification.WebSites.Web.UserAccountWrappers
{
    using System.Web;
    using System.Web.Security;
    using PushNotification.WebSites.Web.Infrastructure;
    using PushNotification.WebSites.Web.Infrastructure.Helpers;

    public static class MembershipHelper
    {
        private static IUserPrivilegesRepository userPrivilegesRepository;

        public static string UserName
        {
            get
            {
                string ticketValue = null;

                var cookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
                if (cookie != null)
                {
                    ticketValue = cookie.Value;
                }

                if (!string.IsNullOrEmpty(ticketValue))
                {
                    FormsAuthenticationTicket ticket;

                    try
                    {
                        ticket = FormsAuthentication.Decrypt(ticketValue);
                    }
                    catch
                    {
                        return string.Empty;
                    }

                    if (ticket != null)
                    {
                        var identity = new FormsIdentity(ticket);

                        return identity.Name;
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        private static IUserPrivilegesRepository UserPrivilegesRepository
        {
            get { return userPrivilegesRepository ?? (userPrivilegesRepository = new PrivilegesTableServiceContext()); }
        }

        public static bool IsUserAdmin()
        {
            return IsUserLoggedIn() && UserPrivilegesRepository.HasUserPrivilege(Membership.GetUser(UserName).ProviderUserKey.ToString(), PrivilegeConstants.AdminPrivilege);
        }

        public static bool IsUserLoggedIn()
        {
            return !string.IsNullOrWhiteSpace(UserName);
        }
    }
}