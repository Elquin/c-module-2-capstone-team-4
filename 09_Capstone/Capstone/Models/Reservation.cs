using System;
using System.Collections.Generic;
using System.Text;

namespace Capstone.Models
{
    public class Reservation
    {
        public int Id { get; private set; }
        public int SiteId { get; private set; }
        public string Name { get; private set; }
        public DateTime FromDate { get; private set; }
        public DateTime ToDate { get; private set; }
        public DateTime CreateDate { get; private set; }

        public Reservation CreateReservation(DateTime fromDate, DateTime toDate, string name)
        {
            Reservation reservation = new Reservation();

            // TODO Create reservation

            return reservation;
        }
    }
}
