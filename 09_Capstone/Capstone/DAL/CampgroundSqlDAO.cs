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

        // TODO Unit testing for advanced search
        public List<Site> GetAvailableReservations(Campground campground, DateTime fromDate, DateTime toDate, int maxOccupancy, bool mustBeAccessible, int rvLength, bool needsUtilities)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sqlString = @"SELECT TOP 5 * FROM site
                                        WHERE campground_id = @campgroundId
                                        AND site_id NOT IN (SELECT DISTINCT site_id FROM reservation
	                                    WHERE from_date < @toDate AND to_date > @fromDate
	                                    )";
                    if (maxOccupancy > 0)
                    {
                        sqlString += " AND max_occupancy >= @maxOccupancy";
                    }
                    if (mustBeAccessible)
                    {
                        sqlString += " AND accessible = 1";
                    }
                    if (rvLength > 0)
                    {
                        sqlString += " AND max_rv_length >= @rvLength";
                    }
                    if (needsUtilities)
                    {
                        sqlString += " AND utilities = 1";
                    }
                    SqlCommand cmd = new SqlCommand(sqlString, connection);
                    cmd.Parameters.AddWithValue("@campgroundId", campground.Id);
                    cmd.Parameters.AddWithValue("@fromDate", fromDate);
                    cmd.Parameters.AddWithValue("@toDate", toDate);
                    cmd.Parameters.AddWithValue("@maxOccupancy", maxOccupancy);
                    cmd.Parameters.AddWithValue("@rvLength", rvLength);

                    

                    SqlDataReader reader = cmd.ExecuteReader();
                    List<Site> sites = new List<Site>();
                    while (reader.Read())
                    {
                        sites.Add(SqlToSite(reader));
                    }
                    return sites;
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

        public List<Site> GetAvailableReservations(Campground campground, DateTime fromDate, DateTime toDate)
        {
            try
            {
                int maxOccupancy = 0;
                bool mustBeAccessible = false;
                int rvLength = 0;
                bool needsUtilities = false;

                return GetAvailableReservations(campground, fromDate, toDate, maxOccupancy, mustBeAccessible, rvLength, needsUtilities);
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
                    if (!reader.HasRows)
                    {
                        throw new Exception("Campground not in database.");
                    }
                    reader.Read();
                    return ObjectToCampground(reader);
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
