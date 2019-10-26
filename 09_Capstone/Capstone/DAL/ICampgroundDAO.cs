using Capstone.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Capstone.DAL
{
    public interface ICampgroundDAO
    {
        List<Campground> GetCampgroundsInPark(Park park);

        Campground GetCampgroundById(int id);

        // TODO Unit testing for advanced search
        List<Site> GetAvailableReservations(Campground campground, DateTime fromDate, DateTime toDate);
        List<Site> GetAvailableReservations(Campground campground, DateTime fromDate, DateTime toDate, int maxOccupancy, bool mustBeAccessible, int rvLength, bool needsUtilities);
    }
}
