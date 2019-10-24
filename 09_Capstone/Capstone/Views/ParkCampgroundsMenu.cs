using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;
using Capstone.DAL;
using Capstone.Models;

namespace Capstone.Views
{
    public class ParkCampgroundsMenu : CLIMenu
    {
        private readonly Park park;

        const string Command_SearchForReservation = "1";
        const string Command_Return = "2";

        public ParkCampgroundsMenu(IParkDAO parkDAO, ICampgroundDAO campgroundDAO, ISiteDAO siteDAO, IReservationDAO reservationDAO, Park park) : base(parkDAO, campgroundDAO, siteDAO, reservationDAO)
        {
            this.park = park;
            List<Campground> campgrounds = campgroundDAO.GetCampgroundsInPark(park);
            string campgroundTable = "";
            foreach (Campground cg in campgrounds)
            {
                campgroundTable += $"\n#{cg.Id,-4}{cg.Name,-21}{CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(cg.OpenFromMonth),-12}{CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(cg.OpenToMonth),-12}{cg.DailyFee:C}";
            }
            Title = $@"Park Campgrounds
{park.Name}

     Name                 Open        Close       Daily Fee{campgroundTable}";

            // TODO Implement this bonus thing
            menuOptions.Add(Command_SearchForReservation, "Search for Reservation");
            menuOptions.Add(Command_Return, "Return to Previous Screen");
        }

        protected override bool ExecuteSelection(string choice)
        {
            while (true)
            {
                switch (choice.ToLower())
                {
                    case Command_Return:
                        Console.Clear();
                        return false;

                    case Command_SearchForReservation:
                        AvailableReservationSearch();
                        return false;
                    default:
                        Console.WriteLine("The command provided was not a valid command, please try again.");
                        break;
                }
            }
        }

        private void AvailableReservationSearch()
        {
            // TODO Implement this
            Console.WriteLine("Which campground (enter 0 to cancel)?");
            string choice = Console.ReadLine();
            if (choice == "0")
            {
                Console.Clear();
                return;
            }

            // TODO Try catch
            Campground campground = campgroundDAO.GetCampgroundById(int.Parse(choice));

            bool validDates = false;
            DateTime fromDate = DateTime.Now; // TODO Should this really be DateTime.Now?
            DateTime toDate = DateTime.Now; // TODO Should this really be DateTime.Now?
            while (!validDates)
            {
                Console.WriteLine("What is the arrival date?");
                // TODO Try catch or TryParse
                fromDate = DateTime.Parse(Console.ReadLine());
                Console.WriteLine("What is the departure date?");
                toDate = DateTime.Parse(Console.ReadLine());

                validDates = toDate >= fromDate;
                if (!validDates)
                {
                    Console.WriteLine("Departure date must be after arrival date.");
                }
            }

            List<Site> sites = campgroundDAO.GetAvailableReservations(campground, fromDate, toDate);

            if (sites.Count > 0)
            {
                Console.WriteLine("Results Matching Your Search Criteria");
                Console.WriteLine("Site No.   Max Occup.  Accessible?  Max RV Length   Utility   Cost");
                foreach (Site currentSite in sites)
                {
                    string accessibleDisplay = currentSite.Accessible ? "Yes" : "No";
                    string maxRvLengthDisplay = currentSite.MaxRVLength > 0 ? currentSite.MaxRVLength.ToString() : "N/A";
                    string utilityDisplay = currentSite.Utilities ? "Yes" : "N/A";
                    Console.WriteLine($"{currentSite.SiteNumber,-11}{currentSite.MaxOccupancy,-12}{accessibleDisplay,-13}{maxRvLengthDisplay,-16}{utilityDisplay,-10}{campground.DailyFee * (toDate - fromDate).Days + 1:C}");
                }

                Console.WriteLine("Which site should be reserved (enter 0 to cancel)");
                choice = Console.ReadLine();
                if (choice == "0")
                {
                    Console.Clear();
                    return;
                }

                // TODO Try catch
                Site site = siteDAO.GetSiteById(int.Parse(choice));

                Console.WriteLine("What name should the reservation be made under?");
                string reservationName = Console.ReadLine();
                // TODO Do we check if reservationName is empty?

                Reservation reservation = new Reservation(site.Id, reservationName, fromDate, toDate, DateTime.Now);
                reservation.Id = reservationDAO.CreateReservation(reservation);

                Console.WriteLine($"The reservation has been made and the confirmation id is {reservation.Id}");

                Console.ReadKey();
                Console.Clear();
            }
            else
            {
                Console.WriteLine("No results match your criteria.");
                // TODO Should we exit somewhere specific here?
            }
        }
    }
}
