using Native.Core.Model;
using Native.Core.Sqldal;
using Native.Sdk.Cqp.EventArgs;
using Native.Sdk.Cqp.Interface;
using Native.Sdk.Cqp.Model;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;


namespace Native.Core.Envent
{
    public class Event_GroupMessage : IGroupMessage
    {

        Rk_boss rk_Boss = new Rk_boss();
        BaseInfo baseInfo = new BaseInfo();
        Rk_user rk_User = new Rk_user();
        public void GroupMessage(object sender, CQGroupMessageEventArgs e)
        {
            // 获取 At 某人对象
            CQCode cqat = e.FromQQ.CQCode_At();
            //获得消息
            string message = e.Message;
            //分开报刀的字符串，去除空白
            string[] str = Regex.Split(message, "报刀", RegexOptions.IgnorePatternWhitespace);
            string str2 = str[0];
            if (str2.Equals("[CQ:at,qq=2135189439] ")&&message.IndexOf("报刀")>=0)
            {
                if (str[1].Equals(""))
                {
                    e.FromGroup.SendGroupMessage(cqat, "数据错误");
                }
                else
                {
                    //获得报刀的伤害并转换为整型
                    int Hurt = Convert.ToInt32(str[1]);
                    //string constr = "Server=.;DataBase=CQAenlly;Integrated Security=True";
                    // 建立SqlConnection对象
                    //SqlConnection con = new SqlConnection(constr);
                    // 打开连接
                    //con.Open();
                    //SqlConnection con = dButil.SqlOpen();//打开//储存需要执行的sql语句数据库

                    //string str_select_hurt = "select Bos_id,Bos_blood from Rk_boss where Bos_hit=0";

                    //获得qq群名片
                    string str_caid = e.FromGroup.GetGroupMemberInfo(e.FromQQ.Id).Card;
                    if (str_caid.Equals(""))
                    {
                        //获得QQ昵称
                        str_caid = e.FromGroup.GetGroupMemberInfo(e.FromQQ.Id).Nick;
                    }

                    rk_User.Rk_name = str_caid;//获得玩家的群名片或者昵称
                    rk_User.Rk_hurt = Hurt;//获得伤害
                    rk_User.Rk_time = DateTime.Now.ToString();//获得当前时间
                    int n = baseInfo.InsertRk_user(rk_User);//执行插入记录
                    if (n <= 0)//判断玩家的出刀是否存储到了记录表中
                    {
                        e.FromGroup.SendGroupMessage("执行录入失败！");
                    }

                    int Bos_id = 1, Bos_blood = 0;//初始化boss的血和id
                    rk_Boss.Bos_hit = 0;//正在打的boss判断参数
                                        //查询boss的信息
                    DataSet ds = baseInfo.SelectRk_Boss(rk_Boss);

                    //获得ds中boss的id与boss的血量
                    Bos_id = Convert.ToInt32(ds.Tables[0].Rows[0]["Bos_id"].ToString());
                    Bos_blood = Convert.ToInt32(ds.Tables[0].Rows[0]["Bos_blood"].ToString());

                    if (Bos_blood - Hurt < 0)//出刀伤害足够打死该boss
                    {
                        //计算多余的伤害进行存储
                        rk_Boss.Bos_blood = Hurt - Bos_blood;
                        rk_Boss.Bos_id = Bos_id + 1;//获得下个bossid

                        //进行更新信息
                        int result = baseInfo.UpdateRK(rk_Boss, false);

                        //判断是否更新执行正常出现错误
                        if (result <= 0)
                        {
                            e.FromGroup.SendGroupMessage("修改数据失败，异常错误代码A1！");
                        }
                    }
                    else
                    {
                        rk_Boss.Bos_blood = Hurt;//出刀伤害进行赋值
                        rk_Boss.Bos_id = Bos_id;//获得bossid
                        int result = baseInfo.UpdateRK(rk_Boss, true);//进行更新boss信息
                        if (result <= 0)//判断是否出错
                        {
                            e.FromGroup.SendGroupMessage("修改数据失败，异常错误代码A2！");
                        }
                    }

                    //查询boss信息
                    ds = baseInfo.SelectRk_Boss(rk_Boss);
                    string str_cq = "出刀人：" + cqat + "\n" + "报刀：" + Hurt + "\n第" + ds.Tables[0].Rows[0]["Bos_week"].ToString() + "周目\n" + ds.Tables[0].Rows[0]["Bos_boss"].ToString() + "王剩余血量：" + ds.Tables[0].Rows[0]["Bos_blood"].ToString();

                    // 往来源群发送一条群消息, 下列对象会合并成一个字符串发送
                    e.FromGroup.SendGroupMessage(str_cq);
                }

                // 设置该属性, 表示阻塞本条消息, 该属性会在方法结束后传递给酷Q
                e.Handler = true;
                /*
                SqlCommand command = new SqlCommand(str_select_hurt, con);//链接数据库储存语句
                SqlDataReader reader = command.ExecuteReader();//执行语句，创建只读储存结果集

                if (reader.Read())
                {
                    Bos_id = Convert.ToInt32(reader["Bos_id"].ToString());
                    Bos_blood = Convert.ToInt32(reader["Bos_blood"].ToString());
                }
                con.Close();

                //获得qq群名片
                string str_caid = e.FromGroup.GetGroupMemberInfo(e.FromQQ.Id).Card;
                if (str_caid.Equals(""))
                {
                    //获得QQ昵称
                    str_caid = e.FromGroup.GetGroupMemberInfo(e.FromQQ.Id).Nick;
                }                

                string str_insert = "insert into Report_knife_user(Rk_name,Rk_hurt,Rk_time) values ('" + str_caid + "','" + Hurt + "','"+DateTime.Now.ToString()+"')";
                SqlCommand cmd = new SqlCommand(str_insert, con);//储存sql语句
                con.Open();
                cmd.ExecuteNonQuery();//执行sql语句
                con.Close();

                if (Bos_blood - Hurt < 0)
                {                    
                    int n = Hurt - Bos_blood;
                    string sql = "update Rk_boss set Bos_blood=0,Bos_hit=1,Bos_week=Bos_week+1 where Bos_hit=0";
                    cmd = new SqlCommand(sql, con);//储存sql语句
                    con.Open();
                    cmd.ExecuteNonQuery();//执行sql语句
                    con.Close();
                    if (Bos_id + 1 > 6)
                    {
                        Bos_id = 1;
                        sql = "update Rk_boss set Bos_blood=6000000 where Bos_id=2";
                        string sq2 = "update Rk_boss set Bos_blood=8000000 where Bos_id=3";
                        string sq3 = "update Rk_boss set Bos_blood=10000000 where Bos_id=4";
                        string sq4 = "update Rk_boss set Bos_blood=12000000 where Bos_id=5";
                        string sq5 = "update Rk_boss set Bos_blood=20000000 where Bos_id=6";
                        cmd = new SqlCommand(sql+sq2+sq3+sq4+sq5, con);//储存sql语句
                        con.Open();
                        cmd.ExecuteNonQuery();//执行sql语句
                        con.Close();
                    }
                    sql = "update Rk_boss set Bos_blood=Bos_blood-" + n + " ,Bos_hit=0  where Bos_id=" + Bos_id + "+1";
                    cmd = new SqlCommand(sql, con);//储存sql语句
                    con.Open();
                    cmd.ExecuteNonQuery();//执行sql语句
                    con.Close();
                }
                else
                {
                    string sql = "update Rk_boss set Bos_blood=Bos_blood-" + Hurt + " where Bos_id=" + Bos_id;
                    cmd = new SqlCommand(sql, con);//储存sql语句
                    con.Open();
                    cmd.ExecuteNonQuery();//执行sql语句
                    con.Close();
                }

                string str_select = "select Bos_boss,Bos_blood,Bos_week from Rk_boss where Bos_hit=0";
                command = new SqlCommand(str_select, con);//链接数据库储存语句
                con.Open();
                reader = command.ExecuteReader();//执行语句，创建只读储存结果集
                string str_cq = "信息未录入";
                if (reader.Read())
                {
                    str_cq = "出刀人：" + cqat + "\n\r" + "报刀：" + Hurt + "\n\r第" + reader["Bos_week"].ToString() + "周目\n\r" + reader["Bos_boss"].ToString() + "王剩余血量：" + reader["Bos_blood"].ToString();
                }
                con.Close();
                */
            }
        }
    }
}
