using System;
using System.Collections.Generic;
using System.Text;

namespace Capstone.Models
{
    public class Reservation
    {
        public int Id { get; private set; }
        public int SiteId { get; private set; }
        public string Name { get; private set; }
        public DateTime FromDate { get; private set; }
        public DateTime ToDate { get; private set; }
        public DateTime CreateDate { get; private set; }
    }
}
