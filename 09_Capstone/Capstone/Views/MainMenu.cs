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
      /s                                                                            
    .oNs`          o                                                                           `y-  
   sNMMNso`      +MMh.                                                                       `sNMMd.
`mNNMMMMMMN-:` .sMMMNd/   -hh-                                   -`    `-                   /dMMMMMm
yhMMMMMMMMddh`:+mMMMh-`/`:yMN:                                  `+/:  :/+`                 ./hNMMMMm
MMMMMMMMMMMM.-ymMMMMMho/+oMMNs`                                  -+d++d+-                  /dMMMMMMm
MMMMMMMMMMMMMMMMMMMMMMMMMMMMMh:.                                   :MM/                    :yMMMMMMm
MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMm-`                -ossydmdho.       .MMNmNNNNdy-           `odMMMMMMm
MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMmm-              :oMMMMMMMMMm-       .NMMMMMMMMN`           /MMMMMMMMm
MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMN:`:          ``.dMMMMNNNmMNd`        .dNNyysyMd`          /mMMMMMMMMm
MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNs`/        `oNMNh-do   ss+d         -yy    .ys         `+NMMMMMMMMm
MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMN/dy```.``.`..mm:.-ss.`.y-`y-``...`.`.yo.``..+y.``.`..`.omMMMMMMMMMm
MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMmmmmNmmmmmdMmmmNmNNmNNmdNNmdmmddmmmNNmdmdmNNmmmmdmmmmmMMMMMMMMMMm
MMMMMMMMMMMMMMMMMMMMMMMMMM __  __       _         __  __                  MMMMMMMMMMMMMMMMMMMMMMMMMm
MMMMMMMMMMMMMMMMMMMMMMMMMM|  \/  | __ _(_)_ __   |  \/  | ___ _ __  _   _ MMMMMMMMMMMMMMMMMMMMMMMMMm
MMMMMMMMMMMMMMMMMMMMMMMMMM| |\/| |/ _` | | '_ \  | |\/| |/ _ \ '_ \| | | |MMMMMMMMMMMMMMMMMMMMMMMMMm
MMMMMMMMMMMMMMMMMMMMMMMMMM| |  | | (_| | | | | | | |  | |  __/ | | | |_| |MMMMMMMMMMMMMMMMMMMMMMMMMm
MMMMMMMMMMMMMMMMMMMMMMMMMM|_|  |_|\__,_|_|_| |_| |_|  |_|\___|_| |_|\__,_|MMMMMMMMMMMMMMMMMMMMMMMMMm
MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNNNNNNNNNNNNNNNNNNNNMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMm";
        }
        protected override bool ExecuteSelection(string choice)
        {
            while (true)
            {
                try
                {
                    Console.Clear();
                    switch (choice.ToLower())
                    {
                        case Command_Quit:
                            return false;

                        default:
                            // Check whether the option entered was a park ID
                            if (menuOptions.ContainsKey(choice))
                            {
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
                                Console.WriteLine("The command provided was not a valid command; Please try again.");
                            }
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    Console.ReadKey();
                }

            }
        }
    }
}
