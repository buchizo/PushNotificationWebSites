namespace PushNotification.WebSites.Web.Controllers
{
    using System.Web.Mvc;

    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            this.ViewBag.Message = "Home";

            return this.View();
        }
    }
}
