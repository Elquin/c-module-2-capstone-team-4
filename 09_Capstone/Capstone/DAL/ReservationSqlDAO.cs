﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using Capstone.Models;

namespace Capstone.DAL
{
    public class ReservationSqlDAO : IReservationDAO
    {
        private string connectionString;

        public ReservationSqlDAO(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public int? CreateReservation(Reservation reservation)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand(@"INSERT INTO reservation (site_id, name, from_date, to_date, create_date)
                                                    VALUES(@siteid, @name, @from_date, @to_date, @create_date)
                                                    SELECT @@IDENTITY", connection);
                    cmd.Parameters.AddWithValue("@siteid", reservation.SiteId);
                    cmd.Parameters.AddWithValue("@name", reservation.Name);
                    cmd.Parameters.AddWithValue("@from_date", reservation.FromDate);
                    cmd.Parameters.AddWithValue("@to_date", reservation.ToDate);
                    cmd.Parameters.AddWithValue("@create_date", reservation.CreateDate);
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (SqlException)
            {
                return null;
            }
            catch (Exception)
            {
                return null;
            }


        }

        public List<Reservation> GetNext30DaysParkReservations(Park park)
        {
            try
            {
                List<Reservation> reservations = new List<Reservation>();
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand(@"SELECT r.*, s.site_number, cg.name AS campground_name FROM reservation r
                                                    JOIN site s ON s.site_id = r.site_id
                                                    JOIN campground cg ON cg.campground_id = s.campground_id
                                                    JOIN park p ON p.park_id = cg.park_id
                                                    WHERE p.park_id = @parkId
                                                    AND r.from_date <= @dateLimit
                                                    AND r.from_date >= @dateNow
                                                    ORDER BY r.from_date, r.to_date, cg.name",
                                                    connection);
                    cmd.Parameters.AddWithValue("@parkId", park.Id);
                    cmd.Parameters.AddWithValue("@dateLimit", DateTime.Now.Date.AddDays(30));
                    cmd.Parameters.AddWithValue("@dateNow", DateTime.Now.Date);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Reservation reservation = SqlToReservation(reader);
                        reservation.SiteNumber = Convert.ToInt32(reader["site_number"]);
                        reservation.CampgroundName = Convert.ToString(reader["campground_name"]);
                        reservations.Add(reservation);
                    }
                }
                return reservations;
            }
            catch (SqlException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }


        //Reservation
        public Reservation GetReservationById(int id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand("SELECT * FROM reservation WHERE reservation_id = @reservationid", connection);
                    cmd.Parameters.AddWithValue("@reservationid", id);
                    SqlDataReader reader = cmd.ExecuteReader();
                    reader.Read();
                    return SqlToReservation(reader);
                }
            }
            catch (SqlException)
            {
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<Reservation> GetReservationsAtSite(Site site)
        {
            try
            {
                List<Reservation> reservations = new List<Reservation>();
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand("SELECT * From reservation WHERE site_id = @siteid", connection);
                    cmd.Parameters.AddWithValue("@siteid", site.Id);
                    SqlDataReader reader = cmd.ExecuteReader();
                    reader.Read();
                    while (reader.Read())
                    {
                        reservations.Add(SqlToReservation(reader));
                    }
                    return reservations;
                }
            }
            catch (SqlException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
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
