using Capstone.DAL;
using Capstone.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Capstone
{
    public static class Reserve
    {
        public static int? MakeReservation(Site site, string reservationName, DateTime fromDate, DateTime toDate, ICampgroundDAO campgroundDAO, IReservationDAO reservationDAO)
        {
            try
            {
                if (toDate <= fromDate)
                {
                    throw new Exception("End date is less than or equal to start date.");
                }

                Campground campground = campgroundDAO.GetCampgroundById(site.CampgroundId);
                if (campground == null)
                {
                    throw new Exception("No campground found.");
                }

                if (Math.Min(fromDate.Month, toDate.Month) < campground.OpenFromMonth || Math.Max(fromDate.Month, toDate.Month) > campground.OpenToMonth)
                {
                    throw new Exception("Date(s) are outside of campground open months.");
                }

                List<Site> sites = campgroundDAO.GetAvailableReservations(campground, fromDate, toDate);
                bool siteIsValid = false;
                foreach (Site siteItem in sites)
                {
                    if (siteItem.Id == site.Id)
                    {
                        siteIsValid = true;
                        break;
                    }
                }
                if (!siteIsValid)
                {
                    return null;
                }

                Reservation reservation = new Reservation(site.Id, reservationName, fromDate, toDate, DateTime.UtcNow);
                reservation.Id = reservationDAO.CreateReservation(reservation);
                if (reservation.Id == null)
                {
                    throw new Exception("Reservation not created.");
                }

                return reservation.Id;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
