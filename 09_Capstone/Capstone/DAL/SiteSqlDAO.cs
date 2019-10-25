using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using Capstone.Models;

namespace Capstone.DAL
{
    public class SiteSqlDAO : ISiteDAO
    {
        private string connectionString;
        public SiteSqlDAO(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public Site GetSiteByCampgroundSiteNumber(Campground campground, int siteNumber)
        {
            // TODO implement this and call it from the ParkCampgroundsMenu
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand("SELECT * FROM site WHERE campground_id = @campgroundId AND site_number = @siteNumber", connection);
                    cmd.Parameters.AddWithValue("@campgroundId", campground.Id);
                    cmd.Parameters.AddWithValue("@siteNumber", siteNumber);
                    SqlDataReader reader = cmd.ExecuteReader();
                    reader.Read();
                    return SqlToSite(reader);
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

        public Site GetSiteById(int id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand("SELECT * FROM site WHERE site_id = @siteId", connection);
                    cmd.Parameters.AddWithValue("@siteId", id);
                    SqlDataReader reader = cmd.ExecuteReader();
                    // TODO What if no rows are found?
                    reader.Read();
                    return SqlToSite(reader);
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

        public List<Site> GetSitesInCampground(Campground campground)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand("SELECT * FROM site WHERE campground_id = @id", connection);
                    cmd.Parameters.AddWithValue("@id", campground.Id);
                    SqlDataReader reader = cmd.ExecuteReader();
                    List<Site> sites = new List<Site>();
                    while (reader.Read()){
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
