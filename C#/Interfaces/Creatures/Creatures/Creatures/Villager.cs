using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Creatures
{
    internal class Villager : Creature , ITalkable
    {
        public override string GameName()
        {
            return "村民";
        }

        public string TalkTo(Creature target)
        {
            string targetName = target.GameName();
            return $"哈囉 {targetName}！我是村民！";
        }


    }
}
