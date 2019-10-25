using System;
using System.Collections.Generic;
using System.Text;

namespace Capstone.Models
{
    public class Park
    {
        public int Id { get; }
        public string Name { get; }
        public string Location{ get; }
        public DateTime EstablishDate{ get; }
        public int Area{ get; }
        public int Visitors{ get; }
        public string Description{ get; }

        public Park(int id, string name, string location, DateTime establishDate, int area, int visitors, string description)
        {
            Id = id;
            Name = name;
            Location = location;
            EstablishDate = establishDate;
            Area = area;
            Visitors = visitors;
            Description = description;
        }
    }
}
