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
            #region 使用傳統方法
            CastSkill1();
            CastSkill2();
            CastSkill3();
            #endregion

            #region 使用委派方法
            // 因每個技能固定都消耗魔力與播放特效，只要把不同的技能效果傳進來即可
            CastSkill(DoDamage); 
            CastSkill(DoHeal); 
            CastSkill(DoTelePort);
            #endregion
        }

        #region 若技能一直增加，會導致方法Funtion數量爆炸
        public void CastSkill1()
        {
            ConsumeMP();
            PlayEffects();
            DoDamage();
        }

        public void CastSkill2()
        {
            ConsumeMP();
            PlayEffects();
            DoHeal();
        }

        public void CastSkill3()
        {
            ConsumeMP();
            PlayEffects();
            DoTelePort();
        }
        // .... 一直增加技能，Funtion越來越多
        // public void CastSkill4()
        #endregion

        #region 改成委派的方式
        public void CastSkill(SkillDelegate skill)
        {
            ConsumeMP();
            PlayEffects();
            skill(); // 傳入不同的技能
        }
        #endregion

        public void ConsumeMP() => Console.WriteLine("消耗魔力");
        public void PlayEffects() => Console.WriteLine("播放特效");

        // 技能施放後效果
        public void DoDamage() => Console.WriteLine("造成傷害");
        public void DoHeal() => Console.WriteLine("治療回血");
        public void DoTelePort() => Console.WriteLine("進行傳送");    
    }
}
