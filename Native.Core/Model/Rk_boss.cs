using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Native.Core.Model
{
    [Serializable]
    class Rk_boss
    {
        private int bos_id;//boss的id
        private int bos_boss;//第几个boss
        private int bos_blood;//boss的血量
        private int bos_week;//boss的周目
        private int bos_hit;//是否为正在攻打的boss

        public int Bos_id { get => bos_id; set => bos_id = value; }
        public int Bos_boss { get => bos_boss; set => bos_boss = value; }
        public int Bos_blood { get => bos_blood; set => bos_blood = value; }
        public int Bos_week { get => bos_week; set => bos_week = value; }
        public int Bos_hit { get => bos_hit; set => bos_hit = value; }
    }
}
