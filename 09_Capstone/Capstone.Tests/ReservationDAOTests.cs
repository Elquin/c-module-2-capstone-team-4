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
        }

        //[TestMethod]
        //public void CreateReservation()
        //{
        //    // Arrange
        //    int reservationSiteId = 5;
        //    string reservationName = "Tomlin Family";
        //    DateTime reservationFromDate = new DateTime(2019, 06, 10);
        //    DateTime reservationToDate = new DateTime(2019, 06, 15);
        //    DateTime reservationCreateDate = DateTime.Now;
        //    Reservation expectedReservation;
        //    ReservationSqlDAO dao = new ReservationSqlDAO(connectionString);

        //    using (SqlConnection conn = new SqlConnection(connectionString))
        //    {
        //        conn.Open();
        //        SqlCommand cmd = new SqlCommand(@"INSERT INTO reservation (site_id, name, from_date, to_date, create_date)
        //                                            VALUES(@siteid, @name, @from_date, @to_date, @create_date)
        //                                            SELECT @@IDENTITY", conn);
        //        cmd.Parameters.AddWithValue("@siteid", reservationSiteId);
        //        cmd.Parameters.AddWithValue("@name", reservationName);
        //        cmd.Parameters.AddWithValue("@from_date", reservationFromDate);
        //        cmd.Parameters.AddWithValue("@to_date", reservationToDate);
        //        cmd.Parameters.AddWithValue("@create_date", reservationCreateDate);
        //        SqlDataReader sdr = cmd.ExecuteReader();
        //        sdr.Read();
        //        expectedReservation = new Reservation(reservationSiteId, reservationName, reservationFromDate, reservationToDate, reservationCreateDate);
        //        //expectedReservation.Id = Convert.ToInt32(cmd.ExecuteScalar());
        //    }


        //    // Act
        //    int? actualReservationId = dao.CreateReservation(expectedReservation);


        //    // Assert 
        //    Assert.AreEqual(expectedReservation.Id, actualReservationId);
        //}

        //[TestMethod]
        //public void GetNext30DaysParkReservationsTest()
        //{
        //    // Arrange
        //    int parkId = 3;
        //    Park newPark = new Park(3, "Cuyahoga Valley", "Ohio", new DateTime(2000, 10, 11), 32860, 2189849, "Park 3");
        //    DateTime dateLimit = DateTime.Now.AddDays(30);
        //    List<Reservation> expectedReservations = new List<Reservation>();
        //    ReservationSqlDAO dao = new ReservationSqlDAO(connectionString);

        //    using (SqlConnection conn = new SqlConnection(connectionString))
        //    {
        //        conn.Open();
        //        SqlCommand cmd = new SqlCommand(@"SELECT r.*, s.site_number, cg.name AS campground_name FROM reservation r
        //                                            JOIN site s ON s.site_id = r.site_id
        //                                            JOIN campground cg ON cg.campground_id = s.site_id
        //                                            JOIN park p ON p.park_id = cg.park_id
        //                                            WHERE p.park_id = @parkId
        //                                            AND r.from_date <= @dateLimit
        //                                            ORDER BY r.from_date, r.to_date, cg.name", conn);
        //        cmd.Parameters.AddWithValue("@parkId", parkId);
        //        cmd.Parameters.AddWithValue("@dateLimit", dateLimit);
        //        SqlDataReader sdr = cmd.ExecuteReader();
        //        sdr.Read();
        //        while (sdr.Read())
        //        {
        //            expectedReservations.Add(SqlToReservation(sdr));
        //        }
        //    }


        //    // Act
        //    List<Reservation> actualReservations = dao.GetNext30DaysParkReservations(newPark);


        //    // Assert 
        //    Assert.AreEqual(expectedReservations[0].Name, actualReservations[0].Name);
        //}

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
