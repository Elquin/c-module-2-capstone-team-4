using System;
using System.Collections.Generic;
using System.Text;

namespace Capstone.Models
{
    public class Park
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Location{ get; private set; }
        public DateTime EstablishDate{ get; private set; }
        public int Area{ get; private set; }
        public int Visitors{ get; private set; }
        public string Description{ get; private set; }


    }
}
