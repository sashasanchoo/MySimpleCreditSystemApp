using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp
{
    class DataWriter<T>:DataOperations
    {
        public void WriteData(T tmp)
        {
            sqlConnection.Open();
            myPropertyInfo = tmp.GetType().GetProperties();
            valuesArray = new string[myPropertyInfo.Length];
            propertyNamesArray = new string[myPropertyInfo.Length];
            DataAdapterForSQL dataAdapter;
            for (int i = 0; i < myPropertyInfo.Length; i++)
            {
                if (myPropertyInfo[i].PropertyType.Name.Contains("DateTime"))
                {
                    dataAdapter = new DataAdapterForSQL();
                    value = dataAdapter.SQLDateTime(Convert.ToDateTime(myPropertyInfo[i].GetValue(tmp)));
                    value = value.Replace('.', '/');
                }
                else
                {
                    value = Convert.ToString(myPropertyInfo[i].GetValue(tmp));
                }
                valuesArray[i] = "'" + value + "'";
                propertyNamesArray[i] = myPropertyInfo[i].Name;
            }
            values = String.Join(",", valuesArray);
            propertyNames = String.Join(",", propertyNamesArray);
            command = $"insert into[Products] ({propertyNames}) values({values})";
            sqlCommand = new SqlCommand(command, sqlConnection);
            sqlCommand.ExecuteNonQuery();
        }
    }
}
