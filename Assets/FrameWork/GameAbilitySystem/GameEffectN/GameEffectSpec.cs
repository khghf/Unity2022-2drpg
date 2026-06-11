using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFW.GameAbilitySystem.GameEffectN
{
    internal class GameEffectSpec:IEquatable<GameEffectSpec>
    {
        public GameEffectSpec() { }



        public bool Equals(GameEffectSpec other)
        {
            if (ReferenceEquals(null, other)) return false;
            return _EffectType.Equals(other._EffectType);
        }

        public override bool Equals(object obj)
        {
            return Equals((GameEffectSpec)obj);
        }

        public static bool operator ==(GameEffectSpec left, GameEffectSpec right)
        {
            return left.Equals(right);
        }
        public static bool operator !=(GameEffectSpec left, GameEffectSpec right)
        {
            return !left.Equals(right);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }


        /// <summary>
        /// GameEffect原型
        /// </summary>
        internal GameEffect _EffectPrototype;
        /// <summary>
        /// GameEffect类型信息
        /// </summary>
        internal Type _EffectType;
    }
}
