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
        private static List<SortData> sortList;
        public static int elementsPerPage = 10;
        public static string folderPath = AppDomain.CurrentDomain.BaseDirectory + "Results" + "\\";

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

            return PartialView("FlightView", flightCollection.ToPagedList(1, elementsPerPage));
        }


        public PartialViewResult GetFlightViewPage(int page)
        {
            return PartialView("FlightView", flightCollection.ToPagedList(page, elementsPerPage));
        }

        public PartialViewResult GetNewData()
        {
            return PartialView("FlightView", flightCollection.ToPagedList(1, elementsPerPage));
        }



        public ActionResult SaveJSON(JsonData json)
        {
            var filePath = folderPath+"temp.json";
            string text = System.Text.RegularExpressions.Regex.Unescape(json.data);
            System.IO.File.WriteAllText(filePath, text);

            var parsed = JObject.Parse(System.IO.File.ReadAllText(filePath));
            var filter = parsed["meta"]["links"]["self"].ToString().Split('?').Last();
            System.IO.File.Move(filePath, folderPath + filter + ".json");

            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FindJSON(string compareUrl)
        {
            DirectoryInfo d = new DirectoryInfo(folderPath);
            foreach (var file in d.GetFiles("*.json"))
            {
                var fileName = Path.GetFileNameWithoutExtension(file.Name);
                if (fileName == compareUrl)
                {
                    JObject jsonObject = JObject.Parse(System.IO.File.ReadAllText(file.FullName));
                    string currency = jsonObject.SelectTokens("data[0].price").Select(f => f["currency"]).FirstOrDefault().ToString();
                    List<decimal> grandTotal = jsonObject.SelectTokens("data[*].price").Select(f => Math.Round(Convert.ToDecimal(f["grandTotal"]),2)).ToList();
                    int passagers = jsonObject.SelectTokens("data[0].travelerPricings").Select(f => f.Count()).FirstOrDefault();
                    var itineraries = jsonObject.SelectTokens("data[*]").Select(t =>new { segment = t["itineraries"].Select(z=>z["segments"]).ToList()}).ToList();
                    int count = 0;
                    flightCollection= new List<FlightData>();
                    foreach (var segments in itineraries)
                    {
                        FlightData flightData = new FlightData();
                        flightData.departureAirport = segments.segment[0].Select(t => t["departure"]).ToList().Select(g => g["iataCode"]).FirstOrDefault().ToString();
                        flightData.departureDateTime = segments.segment[0].Select(t => t["departure"]).ToList().Select(g => g["at"]).FirstOrDefault().ToString();
                        flightData.transferNumberArrival = segments.segment[0].Select(t => t["departure"]).ToList().Count-1;
                        flightData.destinationAirport = segments.segment[1].Select(t => t["departure"]).ToList().Select(g => g["iataCode"]).FirstOrDefault().ToString();
                        flightData.destinationDateTime = segments.segment[1].Select(t => t["departure"]).ToList().Select(g => g["at"]).FirstOrDefault().ToString();
                        flightData.transferNumberArrival = segments.segment[1].Select(t => t["departure"]).ToList().Count-1;
                        flightData.totalCost = grandTotal[count];
                        flightData.passagers = passagers;
                        flightData.currency = currency;

                        flightCollection.Add(flightData);
                        count++;
                    }

                    flightCollection = flightCollection.OrderBy(x => x.departureDateTime).ToList();

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

                    return PartialView("FlightView", flightCollection.ToPagedList(1, elementsPerPage));
                }
            }
            return Json(new { success = false }, JsonRequestBehavior.DenyGet);
        }



        public int GetElementsPerPage()
        {
            return elementsPerPage;
        }

        public ActionResult SortColumn(string column)
        {
            var columnName = sortList.First(x => x.columnName == column);
            switch (column)
            {
                case "departureAirport":
                    if (columnName.count == 0)
                    {
                        columnName.count++;
                        flightCollection = flightCollection.OrderBy(x => x.departureAirport).ToList();
                    }
                    else if (columnName.count == 1)
                    {
                        columnName.count = 0;
                        flightCollection = flightCollection.OrderByDescending(x => x.departureAirport).ToList();
                    }
                    break;

                case "departureDateTime":
                    if (columnName.count == 0)
                    {
                        columnName.count++;
                        flightCollection = flightCollection.OrderBy(x => x.departureDateTime).ToList();
                    }
                    else if (columnName.count == 1)
                    {
                        columnName.count = 0;
                        flightCollection = flightCollection.OrderByDescending(x => x.departureDateTime).ToList();
                    }
                    break;

                case "destinationAirport":
                    if (columnName.count == 0)
                    {
                        columnName.count++;
                        flightCollection = flightCollection.OrderBy(x => x.destinationAirport).ToList();
                    }
                    else if (columnName.count == 1)
                    {
                        columnName.count = 0;
                        flightCollection = flightCollection.OrderByDescending(x => x.destinationAirport).ToList();
                    }
                    break;

                case "destinationDateTime":
                    if (columnName.count == 0)
                    {
                        columnName.count++;
                        flightCollection = flightCollection.OrderBy(x => x.destinationDateTime).ToList();
                    }
                    else if (columnName.count == 1)
                    {
                        columnName.count = 0;
                        flightCollection = flightCollection.OrderByDescending(x => x.destinationDateTime).ToList();
                    }
                    break;

                case "Passagers":
                    if (columnName.count == 0)
                    {
                        columnName.count++;
                        flightCollection = flightCollection.OrderBy(x => x.passagers).ToList();
                    }
                    else if (columnName.count == 1)
                    {
                        columnName.count = 0;
                        flightCollection = flightCollection.OrderByDescending(x => x.passagers).ToList();
                    }
                    break;

                case "totalCost":
                    if (columnName.count == 0)
                    {
                        columnName.count++;
                        flightCollection = flightCollection.OrderBy(x => x.totalCost).ToList();
                    }
                    else if (columnName.count == 1)
                    {
                        columnName.count = 0;
                        flightCollection = flightCollection.OrderByDescending(x => x.totalCost).ToList();
                    }
                    break;

                case "transferNumberDeparture":
                    if (columnName.count == 0)
                    {
                        columnName.count++;
                        flightCollection = flightCollection.OrderBy(x => x.transferNumberDeparture).ToList();
                    }
                    else if (columnName.count == 1)
                    {
                        columnName.count = 0;
                        flightCollection = flightCollection.OrderByDescending(x => x.transferNumberDeparture).ToList();
                    }
                    break;

                case "transferNumberArrival":
                    if (columnName.count == 0)
                    {
                        columnName.count++;
                        flightCollection = flightCollection.OrderBy(x => x.transferNumberArrival).ToList();
                    }
                    else if (columnName.count == 1)
                    {
                        columnName.count = 0;
                        flightCollection = flightCollection.OrderByDescending(x => x.transferNumberArrival).ToList();
                    }
                    break;
            }


            return PartialView("FlightView", flightCollection.ToPagedList(1, elementsPerPage));
        }

        public void ChangeElementsPerPage(int newElementsNumber)
        {
            elementsPerPage = newElementsNumber;
        }
    }
}