using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using Capstone.Models;

namespace Capstone.DAL
{
    public class ParkSqlDAO : IParkDAO
    {
        private string connectionString;

        public ParkSqlDAO(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public List<Site> GetAvailableReservations(Park park, DateTime fromDate, DateTime toDate)
        {
            try
            {
                int maxOccupancy = 0;
                bool mustBeAccessible = false;
                int rvLength = 0;
                bool needsUtilities = false;

                return GetAvailableReservations(park, fromDate, toDate, maxOccupancy, mustBeAccessible, rvLength, needsUtilities);
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
        
        public List<Site> GetAvailableReservations(Park park, DateTime fromDate, DateTime toDate, int maxOccupancy, bool mustBeAccessible, int rvLength, bool needsUtilities)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Get all campgrounds in park
                    SqlCommand cmd = new SqlCommand(@"SELECT * FROM campground WHERE park_id = @parkId
                                                    AND (
                                                        (open_from_mm = 1 AND open_to_mm = 12) OR
                                                        NOT (open_from_mm > @toMonth AND open_to_mm < @fromMonth)
                                                    )",
                                                    connection);
                    cmd.Parameters.AddWithValue("@parkId", park.Id);
                    cmd.Parameters.AddWithValue("@fromMonth", fromDate.Month);
                    cmd.Parameters.AddWithValue("@toMonth", toDate.Month);
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Campground> campgrounds = new List<Campground>();
                    while (reader.Read())
                    {
                        campgrounds.Add(SqlToCampground(reader));
                    }
                    reader.Close();

                    // Get TOP 5 sites in each campground
                    List<Site> sites = new List<Site>();
                    foreach (Campground campground in campgrounds)
                    {
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
                        cmd = new SqlCommand(sqlString, connection);
                        cmd.Parameters.AddWithValue("@campgroundId", campground.Id);
                        cmd.Parameters.AddWithValue("@fromDate", fromDate);
                        cmd.Parameters.AddWithValue("@toDate", toDate);
                        cmd.Parameters.AddWithValue("@maxOccupancy", maxOccupancy);
                        cmd.Parameters.AddWithValue("@rvLength", rvLength);

                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            Site site = SqlToSite(reader);
                            site.CampgroundName = campground.Name;
                            site.CampgroundDailyFee = campground.DailyFee;
                            sites.Add(site);
                        }
                        reader.Close();

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

        public Park GetParkById(int id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand("SELECT * FROM park WHERE park_id = @parkid", connection);
                    cmd.Parameters.AddWithValue("@parkid", id);
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (!reader.HasRows)
                    {
                        throw new Exception("Park not in database.");
                    }
                    reader.Read();
                    return SqlToPark(reader);
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

        public List<Park> GetParks()
        {
            try
            {
                List<Park> parks = new List<Park>();
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand("SELECT * FROM park ORDER BY name", connection);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        parks.Add(SqlToPark(reader));
                    }
                }
                return parks;
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

        private Campground SqlToCampground(SqlDataReader reader)
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
