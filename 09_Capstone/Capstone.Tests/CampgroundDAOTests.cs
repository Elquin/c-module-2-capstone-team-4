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
        public void GetAvailableReservationTest()
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
            List<Site> list = dao.GetAvailableReservations(newCampground, new DateTime(2019, 03, 05), new DateTime(2019, 03, 11));

            // Assert 
            Assert.AreEqual(5, list.Count); //Top 5 limiting results to 5
        }
    }
}
