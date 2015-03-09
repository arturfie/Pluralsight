using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AdgisticsMotors.Web.Services;
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
            return View();
        }
	}
}