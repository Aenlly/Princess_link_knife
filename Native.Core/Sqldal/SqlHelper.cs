using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Native.Core.Sqldal
{
    class SqlHelper
    {
        SqlConnection con = null;
        SqlCommand cmd;
        SqlDataAdapter adapter;
        public void SqlOpen()
        {
            if (con == null)
            {
                //string conn =ConfigurationManager.ConnectionStrings["constr"].ToString(); //链接数据库
                con = new SqlConnection("Data Source=.;database=CQAenlly;Integrated Security=SSPI");
            }
            if (con.State == System.Data.ConnectionState.Closed)//判断是否打开了数据库
            {
                con.Open();
            }
        }

        //关闭数据库执行方法
        public void SqlClose()
        {
            if (con != null)//数据库如果打开
            {
                con.Close();
            }
        }

        public DataSet SqlSelect(string strName, SqlParameter[] prams)
        {
            this.SqlOpen();//打开数据库
            adapter = CreaAdapter(strName, prams);//调用CreaAdapter方法
            DataSet ds = new DataSet();//定义ds数据存储
            adapter.Fill(ds);//adapter数据添加到ds中
            this.SqlClose();//关闭数据库
            return ds;
        }

        //更新
        public int SqlUpdate(string strName, SqlParameter[] prams)
        {
            int result = 0;
            if (prams != null)//判断是否有参数
            {
                this.SqlOpen();//打开数据库
                cmd = CreaCommand(strName, prams);//将语句与参数传递至CreaCommand方法中
                result = cmd.ExecuteNonQuery();//执行语句
                this.SqlClose();//关闭数据库
            }
            else
            {
                this.SqlOpen();//打开数据库
                cmd = new SqlCommand(strName,con);//将执行命令添加到cmd中
                result = cmd.ExecuteNonQuery();//执行语句
                this.SqlClose();//关闭数据库
            }
            return result;
        }

        public SqlDataAdapter CreaAdapter(string strName, SqlParameter[] prams)
        {
            this.SqlOpen();//打开数据库连接
            adapter = new SqlDataAdapter(strName, con);//创建桥接器对象
            adapter.SelectCommand.CommandType = CommandType.Text;//执行类型为文本类型
            if (prams != null)//判断参数不为空
            {
                //遍历参数并添加到命令对象中
                foreach (SqlParameter parameter in prams)
                    adapter.SelectCommand.Parameters.Add(parameter);
            }
            //加入返回参数
            adapter.SelectCommand.Parameters.Add(new SqlParameter("ReturnValue", SqlDbType.Int, 4,
                ParameterDirection.ReturnValue, false, 0, 0, string.Empty, DataRowVersion.Default, null));
            return adapter;
        }


        /// <summary>
        /// 将参数添加到执行语句中
        /// </summary>
        /// <param name="strName">执行语句</param>
        /// <param name="prams">参数</param>
        /// <returns></returns>
        public SqlCommand CreaCommand(string strName, SqlParameter[] prams)
        {
            this.SqlOpen();//打开数据库连接
            cmd = new SqlCommand(strName, con);//创建SqlCommand对象
            cmd.CommandType = CommandType.Text;//执行类型设置为文本
            if (prams != null)//判断参数不为空
            {
                //遍历参数并添加到命令对象中
                foreach (SqlParameter parameter in prams)
                    cmd.Parameters.Add(parameter);
            }
            //加入方法参数
            cmd.Parameters.Add(new SqlParameter("ReturnValue", SqlDbType.Int, 4,
                ParameterDirection.ReturnValue, false, 0, 0, string.Empty, DataRowVersion.Default, null));
            return cmd;
        }
    }

}
