using System;
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

        public int CreateReservation(Reservation reservation)
        {
            throw new NotImplementedException();
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
                    return ObjectToReservation(reader);
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
                        reservations.Add(ObjectToReservation(reader));
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

        private Reservation ObjectToReservation(SqlDataReader reader)
        {
            // TODO check for nulls in create_date
            return new Reservation(
                Convert.ToInt32(reader["reservation_id"]),
                Convert.ToInt32(reader["site_id"]),
                Convert.ToString(reader["name"]),
                Convert.ToDateTime(reader["from_date"]),
                Convert.ToDateTime(reader["to_date"]),
                Convert.ToDateTime(reader["create_date"])

                );
        }
    }
}
