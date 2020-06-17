using Low_cost_flights.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Web.Mvc;

namespace Low_cost_flights.Controllers
{
    public class HomeController : Controller
    {
        private static List<FlightData> flightCollection;
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetCodes()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "IATA codes.txt");
            string[] codes = System.IO.File.ReadAllLines(path);

            return Json(new { list = codes }, JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult FilterData(List<FlightData> flightList)
        {
            flightCollection = new List<FlightData>();
            if (flightList != null)
            {
                foreach (var item in flightList)
                {
                    item.departureDate = DateTime.ParseExact(item.departureDate, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                    item.returnDate = DateTime.ParseExact(item.returnDate, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                    item.totalCost = Math.Round(item.totalCost, 2);
                    flightCollection.Add(item);
                }
            }
            return PartialView("FlightView", flightCollection);

        }
    }
}