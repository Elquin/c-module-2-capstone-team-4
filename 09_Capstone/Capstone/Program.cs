using Capstone.DAL;
using Capstone.Models;
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

            Park park = parkDao.GetParkById(1);

            Console.WriteLine($"{park.Name}");

            Console.WriteLine($"Test{campgroundDao.GetCampgroundById(6).Name}");

            List<Campground> campgrounds = campgroundDao.GetCampgroundsInPark(park);
            foreach (Campground cg in campgrounds)
            {
                Console.WriteLine(cg.Name);
            }

            // TODO Remove all this
            List<Park> parks = parkDao.GetParks();

            foreach (Park park22 in parks)
            {
                Console.WriteLine(park22.Name);
            }

            Console.ReadKey();
        }
    }
}
