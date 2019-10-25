using Capstone.DAL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Capstone.Models
{
    public class Campground 
    {
        public int Id { get; }
        public int ParkId { get; }
        public string Name { get; }
        public int OpenFromMonth { get; }
        public int OpenToMonth { get; }
        public decimal DailyFee { get; }

        

        public Campground(int id, int parkId, string name, int openFromMonth, int openToMonth, decimal dailyFee)
        {
            Id = id;
            ParkId = parkId;
            Name = name;
            OpenFromMonth = openFromMonth;
            OpenToMonth = openToMonth;
            DailyFee = dailyFee;
        }
    }
}
