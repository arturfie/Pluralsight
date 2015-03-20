using System.Web.Mvc;
using AdgisticsMotors.Web.Services.Interfaces;

namespace AdgisticsMotors.Web.Controllers
{
    //Create BaseController
    //Implement IoC Container

    public class HomeController : Controller
    {
        private readonly IReportsService _reportsService;

        public HomeController() { }

        public HomeController(IReportsService reportsService)
        {
            _reportsService = reportsService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult TopPerformingDealerShips()
        {
            var model = _reportsService.TopPerformingDealerships();

            return View(model);
        }

        public ActionResult LowStockDealerShips()
        {
            var model = _reportsService.LowStockDealerships();

            return View(model);
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.ExceptionHandled)
            {
                return;
            }
            filterContext.Result = new ViewResult
            {
                ViewName = "~/Views/Shared/Error.cshtml",
                ViewData = new ViewDataDictionary(filterContext.Exception)
            };
            filterContext.ExceptionHandled = true;
        }
    }
}