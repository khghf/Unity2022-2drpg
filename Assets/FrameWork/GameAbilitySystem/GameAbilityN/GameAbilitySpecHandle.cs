using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFW.GameAbilitySystem.NGameAbility
{
    sealed class GameAbilitySpecHandleGenerator
    { 
        private static int _Handle=0;
        internal static int GenerateNewHandle()
        {
            return ++_Handle;
        }
    }

    /// <summary>
    /// FGameAbilitySpec的唯一句柄(自增句柄)
    /// </summary>
    public struct GameAbilitySpecHandle:IEquatable<GameAbilitySpecHandle>
    {
        public static readonly GameAbilitySpecHandle Invalid=new GameAbilitySpecHandle();


        //public GameAbilitySpecHandle(GameAbilitySpecHandle specHandle)
        //{
        //    this=Invalid;
        //}

        /// <summary>
        /// 生成一个新的句柄并返回
        /// </summary>
        /// <returns></returns>
        public static GameAbilitySpecHandle Create() 
        {
            GameAbilitySpecHandle abilitySpecHandle= new GameAbilitySpecHandle();
            abilitySpecHandle._Handle=GameAbilitySpecHandleGenerator.GenerateNewHandle();
            return abilitySpecHandle;
        }








        public bool IsValid()
        {
            return this!=Invalid;
        }

        public bool Equals(GameAbilitySpecHandle other)
        {
            return this._Handle==other._Handle;
        }

        public override bool Equals(object obj)
        {
            return Equals((GameAbilitySpecHandle)obj);
        }
        public static bool operator ==(GameAbilitySpecHandle left,GameAbilitySpecHandle right)
        {
            return left.Equals(right);
        }
        public static bool operator !=(GameAbilitySpecHandle left, GameAbilitySpecHandle right)
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
