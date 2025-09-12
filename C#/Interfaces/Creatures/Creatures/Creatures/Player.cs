using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Creatures
{
    class Player : Creature , ITalkable
    {
        string name = "勇者";
    
        public override string GameName()
        {
            return $"{name}";
        }

        // 不直觀又需要實作介面方法 IAttackable
        //public void Attack(Creature target)
        //{
        //    target.injured(30);
        //}

        public override string Attack(Creature target)
        { 
            string msg = target.injured(30);
            return msg;
        }

        public string TalkTo(Creature target)
        {
            string targetName = target.GameName();
            return $"哈囉 {targetName}！我是{name}！";
        }

    }
}
