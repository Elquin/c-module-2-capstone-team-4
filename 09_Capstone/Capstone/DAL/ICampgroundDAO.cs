using Capstone.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Capstone.DAL
{
    interface ICampgroundDAO
    {
        List<Campground> GetCampgroundsInPark(Park park);

        Campground GetCampgroundById(int id);

        List<Site> GetAvailableReservations(Campground campground);
    }
}
