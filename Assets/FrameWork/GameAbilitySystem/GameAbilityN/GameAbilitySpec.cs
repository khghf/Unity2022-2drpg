using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFW.GameAbilitySystem.NGameAbility
{
    /// <summary>
    /// 对GameAbility的一层包装，存储运行时置于GameAbility外的额外数据
    /// </summary>
    public class GameAbilitySpec
    {
        public static GameAbilitySpec Create<T>()where T:GameAbility
        {
            return Create(typeof(T));
        }
        public static GameAbilitySpec Create(Type abilityType)
        {
            GameAbilitySpec retSpec = new GameAbilitySpec();
            retSpec.Handle=GameAbilitySpecHandle.Create();
            retSpec.AbilityType=abilityType;
            object abilityInstance = Activator.CreateInstance(abilityType);
            if(abilityInstance==null)
            {
                AbilityLog.LogError($"传入的类型[{abilityType.Name}]不是GameAbility的派生类");
                return retSpec;
            }
            retSpec._AbilityPrototype=abilityInstance as GameAbility;
            retSpec._Level=1;

            return retSpec;

        }
        /// <summary>
        /// 句柄
        /// </summary>
        public GameAbilitySpecHandle Handle;
        /// <summary>
        /// 技能原型
        /// </summary>
        internal GameAbility _AbilityPrototype;
        internal Type AbilityType;
        /// <summary>
        /// 技能等级
        /// </summary>
        internal int _Level;
    }
}
