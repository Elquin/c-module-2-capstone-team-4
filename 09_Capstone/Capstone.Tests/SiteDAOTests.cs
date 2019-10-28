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
        // TODO Dates should be dynamic because data is loaded with dynamic dates

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

        [TestMethod]
        public void GetSitesInCampgroundTest()
        {
            // Arrange
            int campgroundId = 2;
            List<Site> siteList = new List<Site>();
            Site newSite;
            SiteSqlDAO dao = new SiteSqlDAO(connectionString);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM site WHERE campground_id = @campgroundId", conn);
                cmd.Parameters.AddWithValue("@campgroundId", campgroundId);

                SqlDataReader sdr = cmd.ExecuteReader();
                while (sdr.Read())
                {
                    newSite = new Site(Convert.ToInt32(sdr["site_id"]), Convert.ToInt32(sdr["campground_id"]), Convert.ToInt32(sdr["site_number"]), Convert.ToInt32(sdr["max_occupancy"]), Convert.ToBoolean(sdr["accessible"]), Convert.ToInt32(sdr["max_rv_length"]), Convert.ToBoolean(sdr["utilities"]));
                    siteList.Add(newSite);
                }
                
            }


            // Act
            Campground newCampground = new Campground(2, 1, "Seawall", 5, 9, (decimal)30.00);
            List<Site> actualSiteList = dao.GetSitesInCampground(newCampground);

            // Assert 
            Assert.AreEqual(siteList[0].Id, actualSiteList[0].Id);
            Assert.AreEqual(siteList.Count, actualSiteList.Count);
            Assert.AreEqual(siteList[1].CampgroundId, actualSiteList[1].CampgroundId);
        }

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