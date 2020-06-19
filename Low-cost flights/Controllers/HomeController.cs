using Low_cost_flights.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using PagedList;
using System.Reflection;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Low_cost_flights.Controllers
{
    public class HomeController : Controller
    {
        private static IList<FlightData> flightCollection;
        static List<SortData> sortList;
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
                flightCollection = flightCollection.OrderBy(x => x.departureDateTime).ToList();
            }
            //flightPagedList = flightList.ToPagedList(1, elementsPerPage);

            if (flightCollection.Count > 0)
            {
                var x = flightCollection.First();
                sortList = new List<SortData>();
                foreach (PropertyInfo item in x.GetType().GetProperties())
                {
                    SortData t = new SortData();
                    t.columnName = item.Name;
                    sortList.Add(t);
                }
            }

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

        public ActionResult SortColumn(string kolumna)
        {
            var testKolumna = sortList.First(x => x.columnName == kolumna);
            switch (kolumna)
            {
                case "departureAirport":
                    if (testKolumna.count == 0)
                    {
                        testKolumna.count++;
                        flightCollection = flightCollection.OrderBy(x => x.departureAirport).ToList();
                    }
                    else if (testKolumna.count == 1)
                    {
                        testKolumna.count = 0;
                        flightCollection = flightCollection.OrderByDescending(x => x.departureAirport).ToList();
                    }
                    break;

                case "departureDateTime":
                    if (testKolumna.count == 0)
                    {
                        testKolumna.count++;
                        flightCollection = flightCollection.OrderBy(x => x.departureDateTime).ToList();
                    }
                    else if (testKolumna.count == 1)
                    {
                        testKolumna.count = 0;
                        flightCollection = flightCollection.OrderByDescending(x => x.departureDateTime).ToList();
                    }
                    break;

                case "destinationAirport":
                    if (testKolumna.count == 0)
                    {
                        testKolumna.count++;
                        flightCollection = flightCollection.OrderBy(x => x.destinationAirport).ToList();
                    }
                    else if (testKolumna.count == 1)
                    {
                        testKolumna.count = 0;
                        flightCollection = flightCollection.OrderByDescending(x => x.destinationAirport).ToList();
                    }
                    break;

                case "destinationDateTime":
                    if (testKolumna.count == 0)
                    {
                        testKolumna.count++;
                        flightCollection = flightCollection.OrderBy(x => x.destinationDateTime).ToList();
                    }
                    else if (testKolumna.count == 1)
                    {
                        testKolumna.count = 0;
                        flightCollection = flightCollection.OrderByDescending(x => x.destinationDateTime).ToList();
                    }
                    break;

                case "Passagers":
                    if (testKolumna.count == 0)
                    {
                        testKolumna.count++;
                        flightCollection = flightCollection.OrderBy(x => x.passagers).ToList();
                    }
                    else if (testKolumna.count == 1)
                    {
                        testKolumna.count = 0;
                        flightCollection = flightCollection.OrderByDescending(x => x.passagers).ToList();
                    }
                    break;

                case "totalCost":
                    if (testKolumna.count == 0)
                    {
                        testKolumna.count++;
                        flightCollection = flightCollection.OrderBy(x => x.totalCost).ToList();
                    }
                    else if (testKolumna.count == 1)
                    {
                        testKolumna.count = 0;
                        flightCollection = flightCollection.OrderByDescending(x => x.totalCost).ToList();
                    }
                    break;

                case "transferNumberDeparture":
                    if (testKolumna.count == 0)
                    {
                        testKolumna.count++;
                        flightCollection = flightCollection.OrderBy(x => x.transferNumberDeparture).ToList();
                    }
                    else if (testKolumna.count == 1)
                    {
                        testKolumna.count = 0;
                        flightCollection = flightCollection.OrderByDescending(x => x.transferNumberDeparture).ToList();
                    }
                    break;

                case "transferNumberArrival":
                    if (testKolumna.count == 0)
                    {
                        testKolumna.count++;
                        flightCollection = flightCollection.OrderBy(x => x.transferNumberArrival).ToList();
                    }
                    else if (testKolumna.count == 1)
                    {
                        testKolumna.count = 0;
                        flightCollection = flightCollection.OrderByDescending(x => x.transferNumberArrival).ToList();
                    }
                    break;
            }


            return PartialView("FlightView", flightCollection.ToPagedList(1, elementsPerPage));
        }

        public ActionResult SaveJSON(JsonData json)
        {
            var folderPath = AppDomain.CurrentDomain.BaseDirectory + "Results" + "\\";
            var filePath = folderPath+"temp.json";
            string text = System.Text.RegularExpressions.Regex.Unescape(json.data);
            System.IO.File.WriteAllText(filePath, text);

            var parsed = JObject.Parse(System.IO.File.ReadAllText(filePath));
            var filter = parsed["meta"]["links"]["self"].ToString().Split('?').Last();
            System.IO.File.Move(filePath, folderPath + filter + ".json");

            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FindJSON(string url)
        {

        }
    }
}