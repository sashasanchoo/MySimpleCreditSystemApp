using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Reflection;
using System.Configuration;

namespace MyApp
{
    class DataOperations
    {
        protected SqlConnection sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["DBForMyApp"].ConnectionString);
        protected SqlCommand sqlCommand = null;
        protected string[] valuesArray = null;
        protected string[] propertyNamesArray = null;
        protected PropertyInfo[] myPropertyInfo = null;
        protected string values = string.Empty;
        protected string propertyNames = string.Empty;
        protected string value = string.Empty;
        protected SqlDataReader sqlDataReader = null;
        protected string command = string.Empty;
        public const int monthesInYear = 12;
    }
}
