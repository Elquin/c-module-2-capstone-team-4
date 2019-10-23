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

        public Park GetParkById(int id)
        {
            throw new NotImplementedException();
        }

        public List<Park> GetParks()
        {
            List<Park> parks = new List<Park>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM park", connection);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    parks.Add(ObjectToPark(reader));
                }
            }
            return parks;
        }

        private Park ObjectToPark(SqlDataReader reader)
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
