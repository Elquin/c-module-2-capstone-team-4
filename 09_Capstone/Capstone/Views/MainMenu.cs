using System;
using System.Collections.Generic;
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
        }
        
        protected override bool ExecuteSelection(string choice)
        {
            while (true)
            {
                string command = Console.ReadLine();

                Console.Clear();

                switch (command.ToLower())
                {
                    case Command_Quit:
                        //Console.WriteLine("");
                        return false;

                    default:
                        // Check whether the option entered was a park ID
                        if (menuOptions.ContainsKey(command))
                        {
                            // TODO Go to the park menu
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
