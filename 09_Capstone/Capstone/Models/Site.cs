using System;
using System.Collections.Generic;
using System.Text;

namespace Capstone.Models
{
    public class Site
    {
        public int Id { get; private set; }
        public int CampgroundId { get; private set; }
        public int SiteNumber { get; private set; }
        public int MaxOccupancy { get; private set; }
        public bool Accessible { get; private set; }
        public int MaxRVLength { get; private set; }
        public bool Utilities { get; private set; }


        public Reservation CreateReservation(DateTime fromDate, DateTime toDate, string name)
        {
            Reservation reservation = new Reservation();

            // TODO Create reservation

            return reservation;
        }
    }
}
