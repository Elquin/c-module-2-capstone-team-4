using System;
using System.Collections.Generic;
using System.Text;
using Capstone.DAL;
using Capstone.Models;

namespace Capstone.Views
{
    public class ParkMenu : CLIMenu
    {
        private readonly Park park;


        const string Command_ViewCampgrounds = "1";
        const string Command_SearchForReservation = "2";
        const string Command_SearchForReservationAdvanced = "3";
        const string Command_ViewNext30DaysReservations = "4";
        const string Command_Return = "5";

        public ParkMenu(IParkDAO parkDAO, ICampgroundDAO campgroundDAO, ISiteDAO siteDAO, IReservationDAO reservationDAO, Park park) : base(parkDAO, campgroundDAO, siteDAO, reservationDAO)
        {
            this.park = park;
            Title = $@"                                                 
                                     .:`./-`                                                        
                                    -++` `/:`                -.`                                    
                                  ./ooo.   -/.`            -+-:/.`                                  
                                 :+oooo/.`  `/:`          :oo+`.//.`                                
                               .+oooooooo+/`  -/.`      -+oooo.  -+/.                               
                              :oooooooooooo. ` `::`    :oooooo/-```/+-`                             
                          .-:+oooooooooooo+`-+-  .:.`./ooooooooo+/: .//`   1.`                                          
             :/-..-::-.-:.:+ooooooooooooooooooo:.:+oooooooooooooooo+/++o+/++-.-:-`                  
          .:+ooo+.  `-oo+- `-/+ooooooooooooooo+/+oooooooo+/+ooooooooooooooooo+/::/:.`               
         :+oooooo+. -+ooo:    `:+ooooooooooooooooo+ooooooo/.-/oooooooooooooooooo+`-//-`             
       -+ooooooooo+:ooo+-`  .`  `-/+oooooooooooooo-:ooooooo+:..:/oooooooooooooooo: `./+/-`                 
    :+ooooooooooo/::/:`.:+ooooooo++++ooooooooooooo+` .+oooooooo++//::+ooooooooooooooooo++oo+/-`      
MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMm
MMMMMMMM ____            _      ___        __                            _   _             MMMMMMMMm
MMMMMMMM|  _ \ __ _ _ __| | __ |_ _|_ __  / _| ___  _ __ _ __ ___   __ _| |_(_) ___  _ __  MMMMMMMMm
MMMMMMMM| |_) / _` | '__| |/ /  | || '_ \| |_ / _ \| '__| '_ ` _ \ / _` | __| |/ _ \| '_ \ MMMMMMMMm
MMMMMMMM|  __/ (_| | |  |   <   | || | | |  _| (_) | |  | | | | | | (_| | |_| | (_) | | | |MMMMMMMMm
MMMMMMMM|_|   \__,_|_|  |_|\_\ |___|_| |_|_|  \___/|_|  |_| |_| |_|\__,_|\__|_|\___/|_| |_|MMMMMMMMm 
MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNNNNNNNNNNNNNNNNNNNNMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMm 

{park.Name}
Location: {park.Location}
Established: {park.EstablishDate:d}
Area: {park.Area:N0} sq km
Annual Visitors: {park.Visitors:N0}

{park.Description}";

            menuOptions.Add(Command_ViewCampgrounds, "View Campgrounds");
            menuOptions.Add(Command_SearchForReservation, "Search for Reservation");
            menuOptions.Add(Command_SearchForReservationAdvanced, "Advanced Search for Reservation");
            menuOptions.Add(Command_ViewNext30DaysReservations, "View Next 30 Days Reservations");
            menuOptions.Add(Command_Return, "Return to Previous Screen");
        }

        protected override bool ExecuteSelection(string choice)
        {
            while (true)
            {
                Console.Clear();

                switch (choice.ToLower())
                {
                    case Command_Return:
                        return false;

                    case Command_SearchForReservation:
                        AvailableReservationSearch(false);
                        return false;

                    case Command_SearchForReservationAdvanced:
                        AvailableReservationSearch(true);
                        return false;

                    case Command_ViewCampgrounds:
                        ParkCampgroundsMenu parkCampgroundsMenu = new ParkCampgroundsMenu(parkDAO, campgroundDAO, siteDAO, reservationDAO, park);
                        parkCampgroundsMenu.Run();
                        return true;
                    case Command_ViewNext30DaysReservations:
                        List<Reservation> upcomingReservations = reservationDAO.GetNext30DaysParkReservations(park);
                        if (upcomingReservations.Count > 0)
                        {
                            Console.WriteLine($"{park.Name} Upcoming Reservations");
                            Console.WriteLine("Campground              Site #  From Date   To Date     Creation Date   Name");
                            foreach (Reservation r in upcomingReservations)
                            {
                                Console.WriteLine($"{r.CampgroundName,-24}{r.SiteNumber,-8}{r.FromDate,-12:d}{r.ToDate,-12:d}{r.CreateDate,-16:d}{r.Name}");
                            }
                        }
                        else
                        {
                            Console.WriteLine("No upcoming reservations.");
                        }
                        Pause("");
                        return true;
                    default:
                        Console.WriteLine("The command provided was not a valid command, please try again.");
                        break;
                }
            }
        }
        
        private void AvailableReservationSearch(bool isAdvancedSearch)
        {
            try
            {
                string choice;

                while (true)
                {
                    bool validDates = false;
                    DateTime fromDate = DateTime.Now;
                    DateTime toDate = DateTime.Now;
                    while (!validDates)
                    {
                        fromDate = GetDate("What is the arrival date?");
                        toDate = GetDate("What is the departure date?");

                        validDates = toDate > fromDate;
                        if (!validDates)
                        {
                            Console.WriteLine("Departure date must be after arrival date.");
                        }
                    }

                    List<Site> sites;
                    if (isAdvancedSearch)
                    {
                        int maxOccupancy = GetInteger("What is the minimum occupancy of the site you need? (0 to ignore)");
                        bool mustBeAccessible = GetBool("Does the site need to be wheelchair accessible? (Y/N)");
                        int rvLength = GetInteger("What is the minimum length needed for the RV on the site? (0 to ignore)");
                        bool needsUtilities = GetBool("Does the site need hookups for utilities? (Y/N)");

                        sites = parkDAO.GetAvailableReservations(park, fromDate, toDate, maxOccupancy, mustBeAccessible, rvLength, needsUtilities);
                    }
                    else
                    {
                        sites = parkDAO.GetAvailableReservations(park, fromDate, toDate);
                    }

                    if (sites.Count > 0)
                    {
                        Console.WriteLine("Results Matching Your Search Criteria");
                        Console.WriteLine("CG#  Campground                Site No.   Max Occup.  Accessible?  Max RV Length   Utility   Cost");
                        foreach (Site currentSite in sites)
                        {
                            string accessibleDisplay = currentSite.Accessible ? "Yes" : "No";
                            string maxRvLengthDisplay = currentSite.MaxRVLength > 0 ? currentSite.MaxRVLength.ToString() : "N/A";
                            string utilityDisplay = currentSite.Utilities ? "Yes" : "N/A";
                            Console.WriteLine($"{currentSite.CampgroundId,-5}{currentSite.CampgroundName,-26}{currentSite.SiteNumber,-11}{currentSite.MaxOccupancy,-12}{accessibleDisplay,-13}{maxRvLengthDisplay,-16}{utilityDisplay,-10}{currentSite.CampgroundDailyFee * ((toDate - fromDate).Days + 1):C}");
                        }

                        int? campgroundId = null;
                        int? siteNumber = null;
                        bool siteIsValid = false;
                        while (!siteIsValid)
                        {
                            campgroundId = GetInteger("What campground should be reserved? (Enter 0 to cancel.)");
                            if (campgroundId == 0)
                            {
                                Console.Clear();
                                return;
                            }
                            siteNumber = GetInteger("Which site should be reserved? (Enter 0 to cancel.)");
                            if (siteNumber == 0)
                            {
                                Console.Clear();
                                return;
                            }

                            // Check if in list of site numbers
                            foreach (Site siteItem in sites)
                            {
                                if (siteItem.SiteNumber == siteNumber && campgroundId == siteItem.CampgroundId)
                                {
                                    siteIsValid = true;
                                    break;
                                }
                            }
                            
                            if (!siteIsValid)
                            {
                                Console.WriteLine("Invalid site. Please try again.");
                            }
                        }

                        Site site = siteDAO.GetSiteByParkSiteNumber(park, (int)siteNumber);

                        if (site == null)
                        {
                            Pause("Site not found.");
                            return;
                        }

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
