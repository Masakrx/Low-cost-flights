namespace Low_cost_flights.Models
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;

    public class FlightData : DbContext
    {
        public string departureAirport { get; set; }
        public DateTime departureDate { get; set; }
        public int transferNumberDeparture { get; set; }
        public string destinationAirport { get; set; }        
        public DateTime returnDate { get; set; }       
        public int transferNumberArrival { get; set; }
        public int passagers { get; set; }
        public string currency { get; set; }
        public decimal totalCost { get; set; }
    }
}