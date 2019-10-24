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

        public ParkMenu(IParkDAO parkDAO, ICampgroundDAO campgroundDAO, ISiteDAO siteDAO, IReservationDAO reservationDAO, Park park) : base(parkDAO, campgroundDAO, siteDAO, reservationDAO)
        {
            this.park = park;
            Title = $@"Park Information Screen
{park.Name}
Location: {park.Location}
Established: {park.EstablishDate}
Area: {park.Area}
Annual Visitors: {park.Visitors:N}";


        }

        protected override bool ExecuteSelection(string choice)
        {
            throw new NotImplementedException();
        }
    }
}
