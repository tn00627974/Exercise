using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DelegateExample
{
    internal class Delegate2
    {
        public delegate void SkillDelegate();

        //private static SkillDelegate _myDelegate;

        public void Main()
        {
            // 因每個既能固定都消耗魔力與播放特效，只要把不同的技能效果傳進來即可
            CastSkill(DoDamage); 
            CastSkill(DoHeal); 
            CastSkill(DoTelePort); 
        }

        public void CastSkill(SkillDelegate skill)
        {
            ConsumeMP();
            PlayEffects();
            skill();
        }

        public void ConsumeMP() => Console.WriteLine("消耗魔力");
        public void PlayEffects() => Console.WriteLine("播放特效");

        // 技能施放後效果
        public void DoDamage() => Console.WriteLine("造成傷害");
        public void DoHeal() => Console.WriteLine("造成傷害");
        public void DoTelePort() => Console.WriteLine("造成傷害");

    
    }
}
