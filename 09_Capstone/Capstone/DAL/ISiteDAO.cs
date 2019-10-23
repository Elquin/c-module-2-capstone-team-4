using Capstone.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Capstone.DAL
{
    interface ISiteDAO
    {
        List<Site> GetSites();

        Site GetSiteById(int id);
    }
}
