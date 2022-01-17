using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp
{
    class DataDeleter:DataOperations
    {
        private string name;
        public DataDeleter(string name)
        {
            this.name = name;
        }
        public void DeleteData()
        {
            sqlConnection.Open();
            command = $"delete from [Products] where Name like '{name}'";
            sqlCommand = new SqlCommand(command, sqlConnection);
            sqlCommand.ExecuteNonQuery();
        }
    }
}
