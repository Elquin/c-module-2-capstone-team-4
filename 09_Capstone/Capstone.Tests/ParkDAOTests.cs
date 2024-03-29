﻿using Capstone.DAL;
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
    public class ParkDAOTests
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
        public void GetParksTest()
        {
            // Arrange
            ParkSqlDAO dao = new ParkSqlDAO(connectionString);


            List<Park> parks = new List<Park>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM park ORDER BY name", conn);

                SqlDataReader sdr = cmd.ExecuteReader();
                while (sdr.Read())
                {
                    parks.Add(SqlToPark(sdr));
                }

            }


            // Act
            List<Park> actualList = dao.GetParks();

            // Assert 
            Assert.AreEqual(actualList[0].Name, parks[0].Name);
            Assert.AreEqual(actualList[1].Name, parks[1].Name);
            Assert.AreEqual(actualList.Count, parks.Count);

        }

        [TestMethod]
        public void GetParkByIdTest()
        {
            // Arrange
            string expectedName = "Acadia";
            Park expectedPark;
            ParkSqlDAO dao = new ParkSqlDAO(connectionString);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM park WHERE name = @name", conn);
                cmd.Parameters.AddWithValue("@name", expectedName);

                SqlDataReader sdr = cmd.ExecuteReader();
                sdr.Read();
                expectedPark = new Park(Convert.ToInt32(sdr["park_id"]), Convert.ToString(sdr["name"]), Convert.ToString(sdr["location"]), Convert.ToDateTime(sdr["establish_date"]), Convert.ToInt32(sdr["area"]), Convert.ToInt32(sdr["visitors"]), Convert.ToString(sdr["description"]));
            }


            // Act
            Park actualPark = dao.GetParkById(expectedPark.Id);


            // Assert 
            Assert.AreEqual(expectedPark.Name, actualPark.Name);
            Assert.AreEqual(expectedPark.Id, actualPark.Id);

            //Assert2
            actualPark = dao.GetParkById(-1);
            Assert.IsNull(actualPark);

        }

        [TestMethod]
        public void GetAvailableReservationsTest()
        {
            // Arrange
            ParkSqlDAO dao = new ParkSqlDAO(connectionString);
            Park park;
            DateTime fromDate = new DateTime(DateTime.Now.Date.Year - 1, 03, 05);
            DateTime toDate = new DateTime(DateTime.Now.Date.Year - 1, 03, 11);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM park WHERE name = 'Acadia'", conn);

                SqlDataReader sdr = cmd.ExecuteReader();
                sdr.Read();
                park = new Park(Convert.ToInt32(sdr["park_id"]), Convert.ToString(sdr["name"]), Convert.ToString(sdr["location"]), Convert.ToDateTime(sdr["establish_date"]), Convert.ToInt32(sdr["area"]), Convert.ToInt32(sdr["visitors"]), Convert.ToString(sdr["description"]));
            }

            // Act
            List<Site> sites = dao.GetAvailableReservations(park, fromDate, toDate);

            // Assert 
            Assert.AreEqual(15, sites.Count); //Top 5 limiting results to 5 at 3 separate sites

            // Arrange
            fromDate = new DateTime(DateTime.Now.Year - 1, 01, 01);
            toDate = new DateTime(DateTime.Now.Year + 1, 12, 31);

            // Act
            sites = dao.GetAvailableReservations(park, fromDate, toDate, 0, true, 21, true);

            // Assert
            Assert.AreEqual(2, sites.Count);
        }

        private Park SqlToPark(SqlDataReader reader)
        {
            return new Park(
                Convert.ToInt32(reader["park_id"]),
                Convert.ToString(reader["name"]),
                Convert.ToString(reader["location"]),
                Convert.ToDateTime(reader["establish_date"]),
                Convert.ToInt32(reader["area"]),
                Convert.ToInt32(reader["visitors"]),
                Convert.ToString(reader["description"])
                );
        }
    }
}
