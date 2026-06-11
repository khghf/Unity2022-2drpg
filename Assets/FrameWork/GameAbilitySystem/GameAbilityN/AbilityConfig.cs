using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFW.GameAbilitySystem.NGameAbility
{
    public struct AbilityConfig
    {

        public AbilityConfig(EAbilityInstantiationPolicy abilityInstantiationPolicy) 
        {
            AbilityInstantiationPolicy = EAbilityInstantiationPolicy.InstanceForPreGAC;
        }
        public EAbilityInstantiationPolicy AbilityInstantiationPolicy;//= EAbilityInstantiationPolicy.InstanceForPreGAC;
    }
    /// <summary>
    /// 用于决定技能实例化时的执行策略，但不管为那种策略在创建对应的FGameAbilitySpec时都会实例化出一个对象，
    /// 后续执行时会根据该实例的实例化策略来执行
    /// </summary>
    public enum EAbilityInstantiationPolicy
    { 
        InstanceForPreGAC,//拥有该技能的每个GAC只实例化一个技能对象,会在创建
        InstacnePreExcute,//每次执行时重新实例化一个技能对象
    }

}
