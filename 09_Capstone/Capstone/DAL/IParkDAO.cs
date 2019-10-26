using Capstone.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Capstone.DAL
{
    public interface IParkDAO
    {
        List<Park> GetParks();

        Park GetParkById(int id);

        // TODO Implement unit tests for reservation searches by Park
        List<Site> GetAvailableReservations(Park park, DateTime fromDate, DateTime toDate);
        List<Site> GetAvailableReservations(Park park, DateTime fromDate, DateTime toDate, int maxOccupancy, bool mustBeAccessible, int rvLength, bool needsUtilities);
    }
}
