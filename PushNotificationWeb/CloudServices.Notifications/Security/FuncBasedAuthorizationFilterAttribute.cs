namespace PushNotification.WebSites.Web.CloudServices.Notifications
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Web;
    using System.Web.Http;
    using System.Web.Http.Controllers;
    using System.Web.Http.Filters;

    internal abstract class FuncBasedAuthorizationFilterAttribute : AuthorizationFilterAttribute
    {
        public abstract Func<HttpActionContext, bool> Filter { get; }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (this.Filter(actionContext))
            {
                base.OnAuthorization(actionContext);
            }
            else
            {
                // HACK: Prevent ASP.NET Forms Authentication to redirect the user to the login page.
                // This thread-safe approach adds a header with the suppression to be read on the 
                // OnEndRequest event of the pipelien. In order to fully support the supression you should have the ASP.NET Module
                // that does this (SuppressFormsAuthenticationRedirectModule).
                var response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                response.Headers.Add(SuppressFormsAuthenticationRedirectModule.SuppressFormsHeaderName, "true");

                throw new HttpResponseException(response);
            }
        }
    }
}
