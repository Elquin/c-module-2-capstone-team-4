using Capstone.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Capstone.DAL
{
    public interface ISiteDAO
    {
        List<Site> GetSitesInCampground(Campground campground);

        Site GetSiteById(int id);
    }
}
