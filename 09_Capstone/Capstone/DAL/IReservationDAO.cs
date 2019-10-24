using Capstone.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Capstone.DAL
{
    public interface IReservationDAO
    {
        List<Reservation> GetReservationsAtSite(Site site);

        Reservation GetReservationById(int id);

        /// <summary>
        /// Adds a new reservation to the DB
        /// </summary>
        /// <param name="reservation">The reservation (minus Id) to be inserted into the DB</param>
        /// <returns>The id of the reservation</returns>
        int CreateReservation(Reservation reservation);
    }
}
