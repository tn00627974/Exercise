using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Creatures
{
    internal class Monster : Creature
    {
        public override string Say() => "嘖嘖嘖，大開殺戒!！";


        public override string GameName()
        {
            return "小白龍";
        }
    }
}
