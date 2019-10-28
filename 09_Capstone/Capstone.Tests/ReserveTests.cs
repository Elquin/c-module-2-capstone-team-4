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
    public class ReserveTests
    {
        // TODO Dates should be dynamic because data is loaded with dynamic dates

        private TransactionScope transaction;
        const string connectionString = "Server=.\\SQLExpress;Database=npcampground;Trusted_Connection=True;";

        [TestInitialize]
        public void Initialize()
        {
            transaction = new TransactionScope();
            string script;
            using (StreamReader sr = new StreamReader(@"..\..\..\..\Capstone.Tests\CampgroundDAOTest_Setup.sql"))
            {
                script = sr.ReadToEnd();
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand(script, connection);
                cmd.ExecuteNonQuery();
            }
        }

        [TestCleanup]
        public void Cleanup()
        {
            transaction.Dispose();
        }

        [TestMethod]
        public void MakeReservationTest()
        {
            // Arrange
            string campgroundName = "Schoodic Woods";
            int siteNumber = 1;
            int? campgroundId = null;
            Site site;
            string reservationName = "TestReservation";
            DateTime expectedFromDate = new DateTime(2019, 10, 20);
            DateTime expectedToDate = new DateTime(2019, 10, 24);
            int? expectedId = null;
            ICampgroundDAO campgroundDAO = new CampgroundSqlDAO(connectionString);
            IReservationDAO reservationDAO = new ReservationSqlDAO(connectionString);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("SELECT campground_id FROM campground WHERE name = @campgroundName", connection);
                cmd.Parameters.AddWithValue("@campgroundName", campgroundName);
                campgroundId = Convert.ToInt32(cmd.ExecuteScalar());

                cmd = new SqlCommand("SELECT * FROM site WHERE campground_id = @campgroundId AND site_number = @siteNumber", connection);
                cmd.Parameters.AddWithValue("@campgroundId", campgroundId);
                cmd.Parameters.AddWithValue("@siteNumber", siteNumber);
                SqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                site = new Site(
                    Convert.ToInt32(reader["site_id"]),
                    Convert.ToInt32(reader["campground_id"]),
                    Convert.ToInt32(reader["site_number"]),
                    Convert.ToInt32(reader["max_occupancy"]),
                    Convert.ToBoolean(reader["accessible"]),
                    Convert.ToInt32(reader["max_rv_length"]),
                    Convert.ToBoolean(reader["utilities"])
                    );
                reader.Close();

                cmd = new SqlCommand("SELECT MAX(reservation_id) FROM reservation", connection);
                expectedId = Convert.ToInt32(cmd.ExecuteScalar()) + 1;
            }

            // Valid reservation
            // Act
            int? actualId = Reserve.MakeReservation(site, reservationName, expectedFromDate, expectedToDate, campgroundDAO, reservationDAO);

            // Assert
            Assert.AreEqual(expectedId, actualId);

            // Query database for reservation created
            int actualSiteId;
            string actualName;
            DateTime actualFromDate;
            DateTime actualToDate;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM reservation WHERE reservation_id = @reservationId", connection);
                cmd.Parameters.AddWithValue("@reservationId", actualId);
                SqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                actualSiteId = Convert.ToInt32(reader["site_id"]);
                actualName = Convert.ToString(reader["name"]);
                actualFromDate = Convert.ToDateTime(reader["from_date"]);
                actualToDate = Convert.ToDateTime(reader["to_date"]);
            }

            Assert.AreEqual(site.Id, actualSiteId);
            Assert.AreEqual(reservationName, actualName);
            Assert.AreEqual(expectedFromDate, actualFromDate);
            Assert.AreEqual(expectedToDate, actualToDate);

            // Reservation dates overlap
            // Act
            actualId = Reserve.MakeReservation(site, reservationName, expectedFromDate, expectedToDate, campgroundDAO, reservationDAO);

            // Assert
            Assert.IsNull(actualId);


            // From date is after to date
            // Arrange
            expectedFromDate = new DateTime(2019, 1, 20);
            expectedToDate = new DateTime(2019, 1, 15);

            // Act
            actualId = Reserve.MakeReservation(site, reservationName, expectedFromDate, expectedToDate, campgroundDAO, reservationDAO);

            // Assert
            Assert.IsNull(actualId);
        }
    }
}
