using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp
{
    class DataReader<T>:DataOperations
    {
        public void ReadData(T tmp, string name)
        {
            sqlConnection.Open();
            myPropertyInfo = tmp.GetType().GetProperties();
            sqlCommand = new SqlCommand();
            propertyNamesArray = new string[myPropertyInfo.Length];
            NameChecker nameChecker = new NameChecker();
            if (nameChecker.CheckName(name) == 0)
            {
                throw new ProductDoesntExist(name);
            }
            command = $"select * from [Products] where Name like '{name}'";
            sqlCommand = new SqlCommand(command, sqlConnection);
            sqlDataReader = sqlCommand.ExecuteReader();
            while (sqlDataReader.Read())
            {
                for (int i = 0; i < myPropertyInfo.Length; i++)
                {
                    myPropertyInfo[i].SetValue(tmp, sqlDataReader[$"{myPropertyInfo[i].Name}"]);
                }
            }

            if (sqlDataReader != null)
            {
                sqlDataReader.Close();
            }
        }
    }
}
