using Capstone.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Capstone.DAL
{
    interface IReservationDAO
    {
        List<Reservation> GetReservations();

        Reservation GetReservationById(int id);
    }
}
