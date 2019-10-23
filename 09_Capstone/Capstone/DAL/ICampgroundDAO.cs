using Capstone.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Capstone.DAL
{
    interface ICampgroundDAO
    {
        List<Campground> GetCampgrounds(Park park);

        Campground GetCampgroundById(int id);
    }
}
