using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFW.GameAbilitySystem.GameEffectN
{
    sealed class GameEffectSpecHandleGenerator
    {
        private static int _Handle = 0;
        internal static int GenerateNewHandle()
        {
            return ++_Handle;
        }
    }

    /// <summary>
    /// FGameEffectSpec的唯一句柄(自增句柄)
    /// </summary>
    public struct ActiveGameEffectHandle : IEquatable<ActiveGameEffectHandle>
    {
        public static readonly ActiveGameEffectHandle Invalid = new ActiveGameEffectHandle();


        //public ActiveGameEffectHandle()
        //{
        //    this=Invalid;
        //}
        /// <summary>
        /// 生成一个新的句柄并返回
        /// </summary>
        /// <returns></returns>
        public static ActiveGameEffectHandle Create()
        {
            ActiveGameEffectHandle abilitySpecHandle = new ActiveGameEffectHandle();
            abilitySpecHandle._Handle=GameEffectSpecHandleGenerator.GenerateNewHandle();
            return abilitySpecHandle;
        }




        public bool IsValid()
        {
            return this!=Invalid;
        }

        public bool Equals(ActiveGameEffectHandle other)
        {
            return this._Handle==other._Handle;
        }

        public override bool Equals(object obj)
        {
            return Equals((ActiveGameEffectHandle)obj);
        }
        public static bool operator ==(ActiveGameEffectHandle left, ActiveGameEffectHandle right)
        {
            return left.Equals(right);
        }
        public static bool operator !=(ActiveGameEffectHandle left, ActiveGameEffectHandle right)
        {
            return !left.Equals(right);
        }

        public override int GetHashCode()
        {
            return _Handle;
        }
        private int _Handle;
    }
}
