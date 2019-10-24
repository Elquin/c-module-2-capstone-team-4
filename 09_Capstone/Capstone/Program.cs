using Capstone.DAL;
using Capstone.Models;
using Capstone.Views;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;

namespace Capstone
{
    class Program
    {
        static void Main(string[] args)
        {
            // Get the connection string from the appsettings.json file
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();

            string connectionString = configuration.GetConnectionString("Project");

            IParkDAO parkDao = new ParkSqlDAO(connectionString);
            ICampgroundDAO campgroundDao = new CampgroundSqlDAO(connectionString);
            ISiteDAO siteDao = new SiteSqlDAO(connectionString);
            IReservationDAO reservationDao = new ReservationSqlDAO(connectionString);

            CLIMenu menu = new MainMenu(parkDao, campgroundDao, siteDao, reservationDao);
            menu.Run();
        }
    }
}
