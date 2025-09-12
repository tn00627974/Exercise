using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Creatures
{
    abstract class Creature
    {
        protected int hp = 100;
        protected int mp = 100;

        // 角色開場白
        public virtual string Say()
        {
            return $"遊戲名稱 : {GameName()}，HP : {hp} | MP : {mp}";
        }

        // 受到傷害
        public string injured(int damage)
        {
            hp -= damage;

            if (hp < 0) hp = 0; // 避免負數

            string msg = $"{GameName()} 受到 {damage} 點傷害，剩餘 HP : {hp}";

            if (hp == 0)
            {
                msg += $"{GameName()} 已經倒下了！";
            }

            return msg;
        }

        public abstract string GameName();

        public virtual string Attack(Creature target)
        {
            // 預設不攻擊 (NPC 不會做任何事)
            return $"攻擊{target.injured}";
        }
    }
}
