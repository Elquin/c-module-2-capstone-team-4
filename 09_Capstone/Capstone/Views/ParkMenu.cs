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
    }
}
