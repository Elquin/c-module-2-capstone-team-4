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
        const string Command_ViewNext30DaysReservations = "3";
        const string Command_Return = "4";

        public ParkMenu(IParkDAO parkDAO, ICampgroundDAO campgroundDAO, ISiteDAO siteDAO, IReservationDAO reservationDAO, Park park) : base(parkDAO, campgroundDAO, siteDAO, reservationDAO)
        {
            this.park = park;
            Title = $@"                                                 
                                                            `/+o/.
                                                         `:+/.  .so.
                                                    .--::/.       .os:`
                                    :-          `.--.               `:+//+/`                        
                                 /y/oy/.     `..                         `://`                     
         ____            _     .___      `.__:```                        _/: _`-.--.             
        |  _ \ __ _ _ __| | __ |_ _|_ __  / _| ___  _ __ _ __ ___   __ _| |_(_) ___``_ __  
        | |_) / _` | '__| |/ /  | || '_ \| |_ / _ \| '__| '_ ` _ \ / _` | __| |/ _ \| '_ \ 
        |  __/ (_| | |  |   <   | || | | |  _| (_) | |  | | | | | | (_| | |_| | (_) | | | |
        |_|   \__,_|_|  |_|\_\ |___|_| |_|_|  \___/|_|  |_| |_| |_|\__,_|\__|_|\___/|_| |_|
                .-:.  ```                     ./.  ``                                  `  
           ``..-`                               .--.....``
          ``                                              ````                                                                            
{park.Name}
Location: {park.Location}
Established: {park.EstablishDate:d}
Area: {park.Area:N0} sq km
Annual Visitors: {park.Visitors:N0}

{park.Description}";

            menuOptions.Add(Command_ViewCampgrounds, "View Campgrounds");
            // TODO Implement search for reservations in a park
            menuOptions.Add(Command_SearchForReservation, "Search for Reservation");
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
                        // TODO Implement search for reservations available in a park
                        return false;
                    case Command_ViewCampgrounds:
                        ParkCampgroundsMenu parkCampgroundsMenu = new ParkCampgroundsMenu(parkDAO, campgroundDAO, siteDAO, reservationDAO, park);
                        parkCampgroundsMenu.Run();
                        return true;
                    case Command_ViewNext30DaysReservations:
                        // TODO BONUS: As a user of the system, I would like the ability to see a list of all upcoming reservations withinthe next 30 days for a selected national park.
                        List<Reservation> upcomingReservations = reservationDAO.GetNext30DaysParkReservations(park);

                        Console.ReadKey();
                        return true;
                    default:
                        Console.WriteLine("The command provided was not a valid command, please try again.");
                        break;
                }
            }
        }
    }
}
