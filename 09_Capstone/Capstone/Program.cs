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



            IParkDAO dao = new ParkSqlDAO(connectionString);

            Console.WriteLine($"{dao.GetParkById(3).Name}");

            // TODO Remove all this
            List<Park> parks = dao.GetParks();

            foreach (Park park in parks)
            {
                Console.WriteLine(park.Name);
            }

            Console.ReadKey();
        }
    }
}
