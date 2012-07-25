namespace PushNotification.WebSites.Web.Controllers
{
    using System;
    using System.Web.Mvc;

    public class ErrorController : Controller
    {
        public ActionResult Index(Exception error)
        {
            var model = new HandleErrorInfo(error, "Error", "Index");

            return this.View("Error", model);
        }
    }
}
