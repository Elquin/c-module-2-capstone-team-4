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

        [TestMethod]
        public void GetReservationByIdTest()
        {
            // Arrange
            int reservationId = 5;
            Reservation expectedReservation;
            ReservationSqlDAO dao = new ReservationSqlDAO(connectionString);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM reservation WHERE reservation_id = @reservationid", conn);
                cmd.Parameters.AddWithValue("@reservationid", reservationId);
                SqlDataReader sdr = cmd.ExecuteReader();
                sdr.Read();
                expectedReservation = SqlToReservation(sdr);
            }


            // Act
            Reservation actualReservation = dao.GetReservationById(reservationId);


            // Assert 
            Assert.AreEqual(expectedReservation.Name, actualReservation.Name);
            Assert.AreEqual(expectedReservation.Id, actualReservation.Id);
            Assert.AreEqual(expectedReservation.FromDate, actualReservation.FromDate);
            Assert.AreEqual(expectedReservation.SiteId, actualReservation.SiteId);
            Assert.AreEqual(expectedReservation.ToDate, actualReservation.ToDate);
        }

        [TestMethod]
        public void CreateReservationTest()
        {
            // Arrange
            int reservationSiteId = 5;
            string reservationName = "Tomlin Family";
            DateTime reservationFromDate = new DateTime(DateTime.Now.Year - 1, 06, 10);
            DateTime reservationToDate = new DateTime(DateTime.Now.Year - 1, 06, 15);
            DateTime reservationCreateDate = DateTime.Now;
            Reservation expectedReservation;
            expectedReservation = new Reservation(reservationSiteId, reservationName, reservationFromDate, reservationToDate, reservationCreateDate);

            ReservationSqlDAO dao = new ReservationSqlDAO(connectionString);

            // Act
            int? actualReservationId = dao.CreateReservation(expectedReservation);

            Reservation actualReservation;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(@"SELECT * FROM reservation WHERE reservation_id = @reservationId", conn);
                cmd.Parameters.AddWithValue("@reservationId", actualReservationId);
                SqlDataReader sdr = cmd.ExecuteReader();
                sdr.Read();
                actualReservation = SqlToReservation(sdr);
            }

            // Assert 
            Assert.AreEqual(expectedReservation.FromDate, actualReservation.FromDate);
            Assert.AreEqual(expectedReservation.Name, actualReservation.Name);
            Assert.AreEqual(expectedReservation.SiteId, actualReservation.SiteId);
            Assert.AreEqual(expectedReservation.ToDate, actualReservation.ToDate);
        }

        [TestMethod]
        public void GetNext30DaysParkReservationsTest()
        {
            // Arrange
            ReservationSqlDAO dao = new ReservationSqlDAO(connectionString);

            Park park;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand(@"SELECT * FROM park WHERE name = 'Arches'", connection);
                SqlDataReader sdr = cmd.ExecuteReader();
                sdr.Read();
                park = new Park(Convert.ToInt32(sdr["park_id"]), Convert.ToString(sdr["name"]), Convert.ToString(sdr["location"]), Convert.ToDateTime(sdr["establish_date"]), Convert.ToInt32(sdr["area"]), Convert.ToInt32(sdr["visitors"]), Convert.ToString(sdr["description"]));
            }

            // Act
            List<Reservation> reservations = dao.GetNext30DaysParkReservations(park);

            // Assert
            Assert.AreEqual(10, reservations.Count);


            // Arrange
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
    }
}
