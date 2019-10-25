using System;
using System.Collections.Generic;
using System.Text;

namespace Capstone.Models
{
    public class Site
    {
        public Site(int id, int campgroundId, int siteNumber, int maxOccupancy, bool accessible, int maxRVLength, bool utilities)
        {
            Id = id;
            CampgroundId = campgroundId;
            SiteNumber = siteNumber;
            MaxOccupancy = maxOccupancy;
            Accessible = accessible;
            MaxRVLength = maxRVLength;
            Utilities = utilities;
        }

        public int Id { get; }
        public int CampgroundId { get; }
        public int SiteNumber { get; }
        public int MaxOccupancy { get; }
        public bool Accessible { get; }
        public int MaxRVLength { get; }
        public bool Utilities { get; }
    }
}
