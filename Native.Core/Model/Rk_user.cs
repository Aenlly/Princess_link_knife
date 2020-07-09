using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Native.Core.Model
{
    [Serializable]
    class Rk_user
    {
        private int rk_id;//出刀的标识符
        private string rk_name;//出刀人
        private int rk_hurt;//出刀伤害
        private string rk_time;//出刀时间
               
        public int Rk_id { get => rk_id; set => rk_id = value; }
        public string Rk_name { get => rk_name; set => rk_name = value; }
        public int Rk_hurt { get => rk_hurt; set => rk_hurt = value; }
        public string Rk_time { get => rk_time; set => rk_time = value; }
    }
}
