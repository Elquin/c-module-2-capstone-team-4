using Capstone.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Capstone.DAL
{
    public interface ISiteDAO
    {
        List<Site> GetSitesInCampground(Campground campground);

        Site GetSiteByCampgroundSiteNumber(Campground campground, int siteNumber);

        Site GetSiteByParkSiteNumber(Park park, int siteNumber);

        Site GetSiteById(int id);
    }
}
