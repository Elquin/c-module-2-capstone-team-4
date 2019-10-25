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
            Title = $@"
         ____            _       ____                                                      _     
        |  _ \ __ _ _ __| | __  / ___|__ _ _ __ ___  _ __   __ _ _ __ ___  _   _ _ __   __| |___ 
        | |_) / _` | '__| |/ / | |   / _` | '_ ` _ \| '_ \ / _` | '__/ _ \| | | | '_ \ / _` / __|
        |  __/ (_| | |  |   <  | |__| (_| | | | | | | |_) | (_| | | | (_) | |_| | | | | (_| \__ \
        |_|   \__,_|_|  |_|\_\  \____\__,_|_| |_| |_| .__/ \__, |_|  \___/ \__,_|_| |_|\__,_|___/
                                             |_|    |___/                                 



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
            try
            {
                Console.WriteLine("Which campground (enter 0 to cancel)?");
                string choice = Console.ReadLine();
                if (choice == "0")
                {
                    Console.Clear();
                    return;
                }

                Campground campground = campgroundDAO.GetCampgroundById(int.Parse(choice));
                if (campground == null)
                {
                    throw new Exception("No campground found.");
                }

                while (true)
                {
                    bool validDates = false;
                    DateTime fromDate = DateTime.Now;
                    DateTime toDate = DateTime.Now;
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

                    if (fromDate.Month < campground.OpenFromMonth || toDate.Month > campground.OpenToMonth)
                    {
                        Console.WriteLine("Campground is not open during these dates.");
                        Console.ReadKey();
                        Console.Clear();
                        return;
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
                            Console.WriteLine($"{currentSite.SiteNumber,-11}{currentSite.MaxOccupancy,-12}{accessibleDisplay,-13}{maxRvLengthDisplay,-16}{utilityDisplay,-10}{campground.DailyFee * ((toDate - fromDate).Days + 1):C}");
                        }

                        Console.WriteLine("Which site should be reserved (enter 0 to cancel)");
                        choice = Console.ReadLine();
                        if (choice == "0")
                        {
                            Console.Clear();
                            return;
                        }

                        // TODO Try catch
                        Site site = siteDAO.GetSiteByCampgroundSiteNumber(campground, int.Parse(choice));

                        string reservationName = "";
                        while (reservationName == "")
                        {
                            Console.WriteLine("What name should the reservation be made under?");
                            reservationName = Console.ReadLine();
                            if (reservationName == "")
                            {
                                Console.WriteLine("Please enter something for the reservation name.");
                            }
                        }

                        int? reservationId = Reserve.MakeReservation(site, reservationName, fromDate, toDate, campgroundDAO, reservationDAO);

                        if (reservationId == null)
                        {
                            Console.WriteLine("The reservation could not be made.");
                        }
                        else
                        {
                            Console.WriteLine($"The reservation has been made and the confirmation id is {reservationId}");
                        }

                        Console.ReadKey();
                        Console.Clear();
                        return;
                    }
                    else
                    {
                        Console.WriteLine("No results match your criteria. Would you like to enter an alternate date range? (Y/N)");
                        choice = Console.ReadLine().ToLower();
                        if (choice != "y")
                        {
                            Console.Clear();
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.ReadKey();
                Console.Clear();
            }
        }
    }
}
