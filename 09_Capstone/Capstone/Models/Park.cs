using System;
using System.Collections.Generic;
using System.Text;

namespace Capstone.Models
{
    public class Park
    {
        public int Id { get; }
        public string Name { get; }
        public string Location{ get; }
        public DateTime EstablishDate{ get; }
        public int Area{ get; }
        public int Visitors{ get; }
        public string Description{ get; }

        public Park(int id, string name, string location, DateTime establishDate, int area, int visitors, string description)
        {
            Id = id;
            Name = name;
            Location = location;
            EstablishDate = establishDate;
            Area = area;
            Visitors = visitors;
            Description = description;
        }

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
