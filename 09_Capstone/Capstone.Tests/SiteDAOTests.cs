using Capstone.DAL;
using Capstone.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Transactions;

namespace Capstone.Tests
{
    [TestClass]
    public class SiteDAOTests
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
        public void GetSiteByCampgroundSiteNumberTest()
        {
            // Arrange
            int siteNumber = 1;
            Site expectedSite;
            Campground newCampground = new Campground(1, 1, "Blackwoods", 1, 12, (decimal)35.00);
            SiteSqlDAO dao = new SiteSqlDAO(connectionString);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM site WHERE campground_id = @number", conn);
                cmd.Parameters.AddWithValue("@number", siteNumber);

                SqlDataReader sdr = cmd.ExecuteReader();
                sdr.Read();
                expectedSite = new Site(Convert.ToInt32(sdr["site_id"]), Convert.ToInt32(sdr["campground_id"]), Convert.ToInt32(sdr["site_number"]), Convert.ToInt32(sdr["max_occupancy"]), Convert.ToBoolean(sdr["accessible"]), Convert.ToInt32(sdr["max_rv_length"]), Convert.ToBoolean(sdr["utilities"]));
            }


            // Act
            Site actualSite = dao.GetSiteByCampgroundSiteNumber(newCampground, 1);


            // Assert 
            Assert.AreEqual(expectedSite.Id, actualSite.Id);
            Assert.AreEqual(expectedSite.CampgroundId, actualSite.CampgroundId);

            //Assert2
            actualSite = dao.GetSiteByCampgroundSiteNumber(newCampground, -1);
            Assert.IsNull(actualSite);

        }

        [TestMethod]
        public void GetSiteByIdTest()
        {
            // Arrange
            int siteId = 1;
            Site expectedSite;
            SiteSqlDAO dao = new SiteSqlDAO(connectionString);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM site WHERE site_id = @siteId", conn);
                cmd.Parameters.AddWithValue("@siteId", siteId);

                SqlDataReader sdr = cmd.ExecuteReader();
                sdr.Read();
                expectedSite = new Site(Convert.ToInt32(sdr["site_id"]), Convert.ToInt32(sdr["campground_id"]), Convert.ToInt32(sdr["site_number"]), Convert.ToInt32(sdr["max_occupancy"]), Convert.ToBoolean(sdr["accessible"]), Convert.ToInt32(sdr["max_rv_length"]), Convert.ToBoolean(sdr["utilities"]));
            }


            // Act
            Site actualSite = dao.GetSiteById(expectedSite.Id);


            // Assert 
            Assert.AreEqual(expectedSite.Id, actualSite.Id);
            Assert.AreEqual(expectedSite.CampgroundId, actualSite.CampgroundId);

            //Assert2
            actualSite = dao.GetSiteById(-1);
            Assert.IsNull(actualSite);

        }

        //[TestMethod]
        //public void GetSitesInCampgroundTest()
        //{
        //    // Arrange
        //    string expectedName = "Acadia";
        //    Park expectedPark;
        //    ParkSqlDAO dao = new ParkSqlDAO(connectionString);

        //    using (SqlConnection conn = new SqlConnection(connectionString))
        //    {
        //        conn.Open();
        //        SqlCommand cmd = new SqlCommand("SELECT * FROM park WHERE name = @name", conn);
        //        cmd.Parameters.AddWithValue("@name", expectedName);

        //        SqlDataReader sdr = cmd.ExecuteReader();
        //        sdr.Read();
        //        expectedPark = new Park(Convert.ToInt32(sdr["park_id"]), Convert.ToString(sdr["name"]), Convert.ToString(sdr["location"]), Convert.ToDateTime(sdr["establish_date"]), Convert.ToInt32(sdr["area"]), Convert.ToInt32(sdr["visitors"]), Convert.ToString(sdr["description"]));
        //    }


        //    // Act
        //    Park actualPark = dao.GetParkById(expectedPark.Id);


        //    // Assert 
        //    Assert.AreEqual(expectedPark.Name, actualPark.Name);
        //    Assert.AreEqual(expectedPark.Id, actualPark.Id);

        //    //Assert2
        //    actualPark = dao.GetParkById(-1);
        //    Assert.IsNull(actualPark);

        //}

        public Site SqlToSite(SqlDataReader reader)
        {
            return new Site(
                Convert.ToInt32(reader["site_id"]),
                Convert.ToInt32(reader["campground_id"]),
                Convert.ToInt32(reader["site_number"]),
                Convert.ToInt32(reader["max_occupancy"]),
                Convert.ToBoolean(reader["accessible"]),
                Convert.ToInt32(reader["max_rv_length"]),
                Convert.ToBoolean(reader["utilities"])
                );
        }

    }
}
