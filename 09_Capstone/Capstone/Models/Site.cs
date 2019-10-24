using System;
using System.Collections.Generic;
using System.Text;

namespace Capstone.Models
{
    public class Site
    {
        public int Id { get; }
        public int CampgroundId { get; }
        public int SiteNumber { get; }
        public int MaxOccupancy { get; }
        public bool Accessible { get; }
        public int MaxRVLength { get; }
        public bool Utilities { get; }


        public Reservation CreateReservation(DateTime fromDate, DateTime toDate, string name)
        {
            

            Reservation reservation = new Reservation(Id, name, fromDate, toDate, DateTime.Now);

            // TODO Create reservation

            return reservation;
        }
    }
}
