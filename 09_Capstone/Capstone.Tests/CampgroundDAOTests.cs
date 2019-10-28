using Capstone.DAL;
using Capstone.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Transactions;

namespace Capstone.Tests
{
    [TestClass]
    public class CampGroundDAOTests
    {
        private TransactionScope transaction;
        const string connectionString = "Server=.\\SQLExpress;Database=npcampground;Trusted_Connection=True;";
        [TestInitialize]
        public void Setup()
        {
            // Begin a transaction
            this.transaction = new TransactionScope();
            string script;
            // Load a script file to setup the db the way we want it
            using (StreamReader sr = new StreamReader(@"..\..\..\..\Capstone.Tests\CampgroundDAOTest_Setup.sql"))
            {
                script = sr.ReadToEnd();
            }

            

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(script, conn);

                cmd.ExecuteNonQuery();
                
            }


        }

        [TestCleanup]
        public void Cleanup()
        {
            // Roll back the transaction
            this.transaction.Dispose();
        }

        [TestMethod]
        public void GetAvailableReservationsTest()
        {
            Campground newCampground;
            // Arrange
            CampgroundSqlDAO dao = new CampgroundSqlDAO(connectionString);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM campground WHERE name = 'Blackwoods'", conn);

                SqlDataReader sdr = cmd.ExecuteReader();
                sdr.Read();
                newCampground = new Campground(Convert.ToInt32(sdr["campground_id"]), Convert.ToInt32(sdr["park_id"]), Convert.ToString(sdr["name"]), Convert.ToInt32(sdr["open_from_mm"]), Convert.ToInt32(sdr["open_to_mm"]), Convert.ToDecimal(sdr["daily_fee"]));
            }


            // Act
            List<Site> sites = dao.GetAvailableReservations(newCampground, new DateTime(2019, 03, 05), new DateTime(2019, 03, 11));

            // Assert 
            Assert.AreEqual(5, sites.Count); //Top 5 limiting results to 5

            // Arrange
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM campground WHERE name = 'Seawall'", conn);

                SqlDataReader sdr = cmd.ExecuteReader();
                sdr.Read();
                newCampground = new Campground(Convert.ToInt32(sdr["campground_id"]), Convert.ToInt32(sdr["park_id"]), Convert.ToString(sdr["name"]), Convert.ToInt32(sdr["open_from_mm"]), Convert.ToInt32(sdr["open_to_mm"]), Convert.ToDecimal(sdr["daily_fee"]));
            }

            // Act
            sites = dao.GetAvailableReservations(newCampground, new DateTime(2019, 01, 01), new DateTime(2019, 12, 31));

            // Assert
            Assert.AreEqual(3, sites.Count);
        }

        [TestMethod]
        public void GetCampgroundByIdTest()
        {

            // Arrange
            string expectedName = "Devil's Garden";
            Campground expectedCampground;
            CampgroundSqlDAO dao = new CampgroundSqlDAO(connectionString);
            
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM campground WHERE name = @name", conn);
                cmd.Parameters.AddWithValue("@name", expectedName);

                SqlDataReader sdr = cmd.ExecuteReader();
                sdr.Read();
                expectedCampground = new Campground(Convert.ToInt32(sdr["campground_id"]), Convert.ToInt32(sdr["park_id"]), Convert.ToString(sdr["name"]), Convert.ToInt32(sdr["open_from_mm"]), Convert.ToInt32(sdr["open_to_mm"]), Convert.ToDecimal(sdr["daily_fee"]));
            }


            // Act
            Campground actualCampground = dao.GetCampgroundById(expectedCampground.Id);


            // Assert 
            Assert.AreEqual(expectedName, actualCampground.Name);
            Assert.AreEqual(expectedCampground.DailyFee, actualCampground.DailyFee);
            Assert.AreEqual(expectedCampground.OpenFromMonth, actualCampground.OpenFromMonth);
            Assert.AreEqual(expectedCampground.OpenToMonth, actualCampground.OpenToMonth);
            Assert.AreEqual(expectedCampground.ParkId, actualCampground.ParkId);


            //Assert2
            actualCampground = dao.GetCampgroundById(-1);
            Assert.IsNull(actualCampground);
        }


        [TestMethod]
        public void GetCampgroundsInParkTest()
        {

            // Arrange
            int parkId = 1;
            List<Campground> campgroundList = new List<Campground>();
            Campground newCampground;
            CampgroundSqlDAO dao = new CampgroundSqlDAO(connectionString);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                //Query parks
                SqlCommand cmd = new SqlCommand("SELECT * FROM campground WHERE park_id = @parkid", conn);
                cmd.Parameters.AddWithValue("@parkid", parkId);

                SqlDataReader sdr = cmd.ExecuteReader();
                while (sdr.Read())
                {
                    newCampground = new Campground(Convert.ToInt32(sdr["campground_id"]), Convert.ToInt32(sdr["park_id"]), Convert.ToString(sdr["name"]), Convert.ToInt32(sdr["open_from_mm"]), Convert.ToInt32(sdr["open_to_mm"]), Convert.ToDecimal(sdr["daily_fee"]));
                    campgroundList.Add(newCampground);
                }
                
            }


            // Act
            Park newPark = new Park(1, "Acadia Maine", "Maine", new DateTime(1919, 02, 26), 47389, 2563129, "Test");
            List<Campground> actualCampgroundList = dao.GetCampgroundsInPark(newPark);


            // Assert 
            Assert.AreEqual(campgroundList[0].Name, actualCampgroundList[0].Name);
            Assert.AreEqual(campgroundList[1].Name, actualCampgroundList[1].Name);
            Assert.AreEqual(campgroundList[2].Name, actualCampgroundList[2].Name);
        }
    }
}
