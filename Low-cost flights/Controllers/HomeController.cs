using Low_cost_flights.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using PagedList;


namespace Low_cost_flights.Controllers
{
    public class HomeController : Controller
    {
        private static IList<FlightData> flightCollection;
        public static int elementsPerPage = 10;
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
                    var tempStringDeparture= item.departureDateTime.Split('T');
                    var tempDateDeparture = tempStringDeparture[0].Split('-');
                    var departureDateTime = tempDateDeparture[2] + "." + tempDateDeparture[1] + "." + tempDateDeparture[0] + ". " + tempStringDeparture[1];

                    var tempStringDestination = item.destinationDateTime.Split('T');
                    var tempDateDestination = tempStringDestination[0].Split('-');
                    var destinationDateTime = tempDateDestination[2] + "." + tempDateDestination[1] + "." + tempDateDestination[0] + ". " + tempStringDestination[1];

                    item.departureDateTime = departureDateTime;
                    item.destinationDateTime = destinationDateTime;
                    item.totalCost = Math.Round(item.totalCost, 2);
                    flightCollection.Add(item);
                }
            }
            //flightPagedList = flightList.ToPagedList(1, elementsPerPage);

            /*if (flightPagedList.Count > 0)
            {
                var x = flightCollection.First();
                //sortList = new List<SortFilter>();
                foreach (PropertyInfo item in x.GetType().GetProperties())
                {
                    SortFilter t = new SortFilter();
                    t.ColumnName = item.Name;
                    sortList.Add(t);
                }
            }*/

            int numberOfElements = -1;
            if (flightCollection != null) numberOfElements = flightCollection.Count;

            int numberOfPages = 0;

            if (numberOfElements == -1) { }
            else
            {
                if (numberOfElements / elementsPerPage == 0)
                {
                    numberOfPages = 1;
                }
                else
                {
                    if (numberOfElements % elementsPerPage == 0)
                    {
                        numberOfPages = numberOfElements / elementsPerPage;
                    }
                    else
                    {
                        numberOfPages = numberOfElements / elementsPerPage + 1;
                    }

                }
            }

            return PartialView("FlightView", flightCollection.ToPagedList(1, elementsPerPage));
        }


        public PartialViewResult GetFlightViewPage(int page)
        {
            return PartialView("FlightView", flightCollection.ToPagedList(page, elementsPerPage));
        }


        public int GetElementsPerPage()
        {
            return elementsPerPage;
        }


        public void ChangeElementsPerPage(int newElementsNumber)
        {
            elementsPerPage = newElementsNumber;
        }

        public PartialViewResult GetNewData()
        {
            return PartialView("FlightView", flightCollection.ToPagedList(1, elementsPerPage));
        }
    }
}