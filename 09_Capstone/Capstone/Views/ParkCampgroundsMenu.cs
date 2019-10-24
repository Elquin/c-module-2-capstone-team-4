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
                Console.Clear();

                switch (choice.ToLower())
                {
                    case Command_Return:
                        return false;

                    case Command_SearchForReservation:
                        // TODO Implement this
                        return false;
                    default:
                        Console.WriteLine("The command provided was not a valid command, please try again.");
                        break;
                }
            }
        }
    }
}
