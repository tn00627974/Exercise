using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Creatures
{
    abstract class Creature
    {
        int hp = 100;
        int mp = 100;

        // 角色開場白
        public virtual string Say()
        {
            return $"遊戲名稱 : {GameName()}，HP : {hp} | MP : {mp}";
        }

        // 受到傷害
        public void injured(int damage)
        {
            hp -= damage;
            if (hp < 0) hp = 0; MessageBox.Show($"{GameName()} 受到 {damage} 點傷害，剩餘 HP : {hp}"); ;
            if (hp ==0) MessageBox.Show($"{GameName()} 已經倒下了！"); ;
        }

        public abstract string GameName();

        public virtual void Attack(Creature target)
        {
            // 預設不攻擊 (NPC 不會做任何事)
        }
    }
}
