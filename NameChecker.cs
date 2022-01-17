using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp
{
    class NameChecker:DataOperations
    {
        private int digit;
        public int CheckName(string name)
        {
            sqlConnection.Open();
            command = $"select count(*) from [Products] where Name like '{name}'";
            sqlCommand = new SqlCommand(command, sqlConnection);
            digit = (Int32)sqlCommand.ExecuteScalar();
            if (sqlDataReader != null)
            {
                sqlDataReader.Close();
            }
            return digit;
        }
    }
}
