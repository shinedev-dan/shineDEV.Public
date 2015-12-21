using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Common.Helpers
{
    public class DbHelper
    {
        public enum Database { Default }
        
        public static SqlConnection CreateOpenConnection(Database db)
        {
            SqlConnection cnn = null;
            switch (db)
            {
                case Database.Default:
                    cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString ?? string.Empty);
                    break;
                default:
                    break;
            }
            cnn.Open();
            return cnn;
        }
    }
}