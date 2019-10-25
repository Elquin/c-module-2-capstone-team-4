using System;
using System.Collections.Generic;
using System.Text;

namespace Capstone.Models
{
    public class Reservation
    {
        public int? Id { get; set; }
        public int SiteId { get; }
        public string Name { get; }
        public DateTime FromDate { get; }
        public DateTime ToDate { get; }
        public DateTime CreateDate { get; }

        public Reservation(int id, int siteId, string name, DateTime fromDate, DateTime toDate, DateTime createDate)
        {
            Id = id;
            SiteId = siteId;
            Name = name;
            FromDate = fromDate;
            ToDate = toDate;
            CreateDate = createDate;
        }

        public Reservation (int siteId, string name, DateTime fromDate, DateTime toDate, DateTime createDate)
        {
            SiteId = siteId;
            Name = name;
            FromDate = fromDate;
            ToDate = toDate;
            CreateDate = createDate;
        }
    }
}
