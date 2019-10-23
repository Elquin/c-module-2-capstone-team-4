using System;
using System.Collections.Generic;
using System.Text;

namespace Capstone.Models
{
    public class Park
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Location{ get; private set; }
        public DateTime EstablishDate{ get; private set; }
        public int Area{ get; private set; }
        public int Visitors{ get; private set; }
        public string Description{ get; private set; }

        public List<Campground> GetCampgrounds()
        {
            List<Campground> campgrounds = new List<Campground> { };

            // TODO Query DB for campgrounds here

            return campgrounds;
        }

        public List<Reservation> SearchAvailableReservations(DateTime arrivalDate, DateTime departureDate)
        {
            List<Reservation> reservations = new List<Reservation>();

            // TODO Query DB for available reservations

            return reservations;
        }
    }
}
