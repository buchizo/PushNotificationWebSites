namespace PushNotification.WebSites.Web.Infrastructure
{
    using System;
    using System.Globalization;
    using System.Web.Mvc;
    using System.Web.Security;
    using PushNotification.WebSites.Web.Infrastructure;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public sealed class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        private IUserPrivilegesRepository userPrivilegesRepository;

        [CLSCompliant(false)]
        public IUserPrivilegesRepository UserPrivilegesRepository
        {
            get
            {
                return this.userPrivilegesRepository ?? (this.userPrivilegesRepository = new PrivilegesTableServiceContext());
            }

            private set
            {
                this.userPrivilegesRepository = value;
            }
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            var cookie = filterContext.HttpContext.Request.Cookies[FormsAuthentication.FormsCookieName];
            var loginUrl = string.Format(CultureInfo.InvariantCulture, "~/Account/LogOn?returnUrl={0}", filterContext.HttpContext.Request.RawUrl);

            if (cookie == null)
            {
                filterContext.Result = new RedirectResult(loginUrl);
            }
            else
            {
                FormsAuthenticationTicket ticket = null;

                try
                {
                    ticket = FormsAuthentication.Decrypt(cookie.Value);
                }
                catch (ArgumentException)
                {
                    filterContext.Result = new RedirectResult(loginUrl);
                }

                if (ticket != null)
                {
                    var userId = Membership.GetUser(new FormsIdentity(ticket).Name).ProviderUserKey.ToString();
                    if (!this.UserPrivilegesRepository.HasUserPrivilege(userId, this.Roles))
                    {
                        filterContext.Result = new RedirectResult("~/Account/Unauthorized");
                    }
                }
            }
        }
    }
}