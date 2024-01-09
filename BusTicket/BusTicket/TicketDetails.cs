using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusTicket
{
    public class TicketDetails
    {
        public int ID { get; set; }
        public string PassengerName { get; set; }
        public string Destination { get; set; }
        public DateTime DepartureDate { get; set; }
        public decimal TicketPrice { get; set; }
    }

}
