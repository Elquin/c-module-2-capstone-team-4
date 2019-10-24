using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Capstone.DAL;
using Capstone.Models;

namespace Capstone.Views
{
    public class MainMenu : CLIMenu
    {
        const string Command_Quit = "Q";

        public MainMenu(IParkDAO parkDAO, ICampgroundDAO campgroundDAO, ISiteDAO siteDAO, IReservationDAO reservationDAO) : base(parkDAO, campgroundDAO, siteDAO, reservationDAO)
        {
            List<Park> parks = parkDAO.GetParks();
            foreach (Park park in parks)
            {
                menuOptions.Add(park.Id.ToString(), park.Name);
            }

            menuOptions.Add(Command_Quit, "Quit");
            Title = $"Main Menu";
        }
        
        protected override bool ExecuteSelection(string choice)
        {
            while (true)
            {
                Console.Clear();

                switch (choice.ToLower())
                {
                    case Command_Quit:
                        //Console.WriteLine("");
                        return false;

                    default:
                        // Check whether the option entered was a park ID
                        if (menuOptions.ContainsKey(choice))
                        {
                            // TODO Try catch
                            ParkMenu parkMenu = new ParkMenu(parkDAO, campgroundDAO, siteDAO, reservationDAO, parkDAO.GetParkById(int.Parse(choice)));
                            parkMenu.Run();
                            return true;
                        }
                        else
                        {
                            Console.WriteLine("The command provided was not a valid command, please try again.");
                        }
                        break;
                }
            }
        }
    }
}
