using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Creatures
{
    class Player : Creature , IAttackable
    {
        public override string GameName()
        {
            return "勇者";
        }

        // 不直觀又需要實作介面方法 IAttackable
        //public void Attack(Creature target)
        //{
        //    target.injured(30);
        //}

        public override void Attack(Creature target)
        {
            target.injured(30);
        }

    }
}
