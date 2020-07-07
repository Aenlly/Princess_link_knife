using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Native.Core.config
{
    class DButil
    {
        public SqlConnection SqlOpen()
        {
            string conn = ConfigurationManager.ConnectionStrings["constr"].ConnectionString; //链接数据库
            SqlConnection con = new SqlConnection(conn);
            con.Open();
            return con;
        }
    }
}
