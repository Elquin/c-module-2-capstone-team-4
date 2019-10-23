using System;
using System.Collections.Generic;
using System.Text;

namespace Capstone.Models
{
    public class Campground
    {
        public int Id { get; private set; }
        public int ParkId{ get; private set; }
        public string Name { get; private set; }
        public int OpenFromMonth{ get; private set; }
        public int OpenToMonth{ get; private set; }
        public decimal DailyFee{ get; private set; }

        public List<Reservation> SearchAvailableReservations(DateTime arrivalDate, DateTime departureDate)
        {
            List<Reservation> reservations = new List<Reservation>();

            // TODO Query DB for available reservations

            return reservations;
        }
    }
}
