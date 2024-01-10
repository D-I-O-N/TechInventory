using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace TechInventory
{
    internal class DataBase
    {
        SqlConnection sqlConnection = new SqlConnection(@"data source=(LocalDb)\MSSQLLocalDB;initial catalog=TechInventory;integrated security=True");

        public void openConnection()
        {
            if (sqlConnection.State == System.Data.ConnectionState.Closed) 
            { 
                sqlConnection.Open();
            }
        }

        public void closeConnection()
        {
            if (sqlConnection.State == System.Data.ConnectionState.Open)
            {
                sqlConnection.Close();
            }
        }

        public SqlConnection getConnection()
        {
            return sqlConnection;
        }
    }
}
