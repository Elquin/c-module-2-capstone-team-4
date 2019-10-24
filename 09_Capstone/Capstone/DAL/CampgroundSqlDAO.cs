using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using Capstone.Models;

namespace Capstone.DAL
{
    public class CampgroundSqlDAO : ICampgroundDAO
    {
        private string connectionString;

        public CampgroundSqlDAO(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public List<Site> GetAvailableReservations(Campground campground)
        {
            // TODO
            throw new NotImplementedException();
        }

        public Campground GetCampgroundById(int id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand("SELECT * FROM campground WHERE campground_id = @campgroundId", connection);
                    cmd.Parameters.AddWithValue("@campgroundId", id);
                    SqlDataReader reader = cmd.ExecuteReader();
                    // TODO What if no rows are found?
                    reader.Read();
                    return ObjectToCampground(reader);
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

        public List<Campground> GetCampgroundsInPark(Park park)
        {
            try
            {
                List<Campground> campgrounds = new List<Campground>();
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand("SELECT * FROM campground WHERE park_id = @parkId", connection);
                    cmd.Parameters.AddWithValue("@parkId", park.Id);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        campgrounds.Add(ObjectToCampground(reader));
                    }
                }
                return campgrounds;
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

        private Campground ObjectToCampground(SqlDataReader reader)
        {
            return new Campground(
                Convert.ToInt32(reader["campground_id"]),
                Convert.ToInt32(reader["park_id"]),
                Convert.ToString(reader["name"]),
                Convert.ToInt32(reader["open_from_mm"]),
                Convert.ToInt32(reader["open_to_mm"]),
                Convert.ToDecimal(reader["daily_fee"])
                );
        }
    }
}
