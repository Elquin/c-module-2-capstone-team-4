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

            menuOptions.Add(Command_Quit, "Quit");  //ivrit font 
            Title = $@"
                 A
                d$b
              .d\$$b.
            .d$i$$\$$b.       
            .d$$$\$$$b
          .d$$@$$$$\$$ib__  __       _         __  __
          .d$@$$\$$$$$@b. \/  | __ _(_)_ __   |  \/  | ___ _ __  _   _
        .d$$$$i$$$\$$$$$$b./| |/ _` | | '_ \  | |\/| |/ _ \ '_ \| | | |
                ###    | |  | | (_| | | | | | | |  | |  __/ | | | |_| |
                ###    |_|  |_|\__,_|_|_| |_| |_|  |_|\___|_| |_|\__,_|";
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
                            Park park = parkDAO.GetParkById(int.Parse(choice));
                            if (park == null)
                            {
                                throw new Exception("Park not found");
                            }
                            ParkMenu parkMenu = new ParkMenu(parkDAO, campgroundDAO, siteDAO, reservationDAO, park);
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
