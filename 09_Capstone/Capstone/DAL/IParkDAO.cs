using Capstone.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Capstone.DAL
{
    interface IParkDAO
    {
        List<Park> GetParks();

        Park GetParkById(int id);
    }
}
