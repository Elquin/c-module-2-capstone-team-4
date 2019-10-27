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
    public class ReservationDAOTests
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
        public void GetReservationsAtSiteTest()
        {
            // Arrange
            int siteId = 1;
            Site newSite = new Site(1, 1, 1, 6, false, 0, false);
            List<Reservation> expectedReservations = new List<Reservation>();
            ReservationSqlDAO dao = new ReservationSqlDAO(connectionString);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * From reservation WHERE site_id = @siteid", conn);
                cmd.Parameters.AddWithValue("@siteid", siteId);
                SqlDataReader sdr = cmd.ExecuteReader();
                sdr.Read();
                while (sdr.Read())
                {
                    expectedReservations.Add(SqlToReservation(sdr));
                }
            }


            // Act
            List<Reservation> actualReservationList = dao.GetReservationsAtSite(newSite);


            // Assert 
            Assert.AreEqual(expectedReservations[0].Name, actualReservationList[0].Name);
            Assert.AreEqual(expectedReservations.Count, actualReservationList.Count);


        }

        private Reservation SqlToReservation(SqlDataReader reader)
        {
            return new Reservation(
                Convert.ToInt32(reader["reservation_id"]),
                Convert.ToInt32(reader["site_id"]),
                Convert.ToString(reader["name"]),
                Convert.ToDateTime(reader["from_date"]),
                Convert.ToDateTime(reader["to_date"]),
                reader["create_date"] is DBNull ? default(DateTime) : Convert.ToDateTime(reader["create_date"])
                );
        }

        //private Site SqlToSite(SqlDataReader reader)
        //{
        //    return new Site(
        //        Convert.ToInt32(reader["site_id"]),
        //        Convert.ToInt32(reader["campground_id"]),
        //        Convert.ToInt32(reader["site_number"]),
        //        Convert.ToInt32(reader["max_occupancy"]),
        //        Convert.ToBoolean(reader["accessible"]),
        //        Convert.ToInt32(reader["max_rv_length"]),
        //        Convert.ToBoolean(reader["utilities"])
        //        );
        //}
    }
}
