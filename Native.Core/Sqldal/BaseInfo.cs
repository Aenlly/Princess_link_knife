using Native.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace Native.Core.Sqldal
{
    class BaseInfo
    {
        SqlHelper sqlHelper=new SqlHelper ();
        public int UpdateRK(Rk_boss rk_Boss, bool b_Update)
        {            
            int result = 0;//进行执行判断
            string strsql;//执行字符串定义
            if (b_Update == false)//判断是否能击杀boss
            {
                //sql执行更新语句
                strsql = " update Rk_boss set Bos_blood = 0, Bos_hit = 1, Bos_week = Bos_week + 1 where Bos_hit = 0";
                result = sqlHelper.SqlUpdate(strsql, null);//执行，无参数
            }
            if (rk_Boss.Bos_id > 6)//判断是否boss为最后一个，是则回调为第一个
            {
                UpdateRk_Boss();//执行boss血量更新
                rk_Boss.Bos_id = 2;//bossid更改记录
            }
            //存储伤害，bossid到参数进行传递到sql语句中
            SqlParameter[] parms =
            {
                new SqlParameter("@Hurt",SqlDbType.VarChar){Value=rk_Boss.Bos_blood },
                new SqlParameter("@Bos_id",SqlDbType.VarChar){Value=rk_Boss.Bos_id }
                };
            //利用bossid进行更新
            strsql = "update Rk_boss set Bos_blood=Bos_blood-@Hurt,Bos_hit=0 where Bos_id=@Bos_id";
            //执行并获得返回值
            result = sqlHelper.SqlUpdate(strsql, parms);

            return result;
        }

        //所以boss击杀执行该方法重置boss血量
        public int UpdateRk_Boss()
        {

            int result = 0;
            //执行boss血量数据更新
            string strsql = "update Rk_boss set Bos_blood=6000000 where Bos_id=2" +
                "update Rk_boss set Bos_blood=8000000 where Bos_id=3" +
                "update Rk_boss set Bos_blood=10000000 where Bos_id=4" +
                "update Rk_boss set Bos_blood=12000000 where Bos_id=5" +
                "update Rk_boss set Bos_blood=20000000 where Bos_id=6";
            //执行
            result = sqlHelper.SqlUpdate(strsql, null);

            return result;
        }

        //获得boss所信息
        public DataSet SelectRk_Boss(Rk_boss rk_Boss)
        {
            //创建ds数据存储
            DataSet ds;
            //创建sql参数为正在击杀boss
            SqlParameter[] prams =
            {
                new SqlParameter("@Bos_hit",SqlDbType.VarChar){ Value=rk_Boss.Bos_hit }
            };
            //sql查询语句
            string strsql = "select * from Rk_boss where Bos_hit=@Bos_hit";
            //执行语句并把数据存储在ds中
            ds = sqlHelper.SqlSelect(strsql, prams);
            return ds;
        }

        //插入玩家出刀信息
        public int InsertRk_user(Rk_user rk_user)
        {
            int result = 0;
            //获得玩家出刀信息存储在参数数组中
            SqlParameter[] parms =
            {
                new SqlParameter("@rk_name",SqlDbType.NVarChar){Value=rk_user.Rk_name},
                new SqlParameter("@rk_hurt",SqlDbType.VarChar){ Value=rk_user.Rk_hurt },
                new SqlParameter("@rk_time",SqlDbType.DateTime){Value=rk_user.Rk_time }
                };
            //定义sql插入执行语句
            string strsql = "insert into Rk_user(Rk_name,Rk_hurt,Rk_time) values (@rk_name,@rk_hurt,@rk_time)";
            //执行插入语句
            result = sqlHelper.SqlUpdate(strsql, parms);
            return result;
        }
    }
}
