using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Creatures
{
    internal interface IAttackable
    {
        public void Attack(Creature target);
    }
}
