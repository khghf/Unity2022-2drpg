using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFW.GameAbilitySystem.NGameAbility
{
    /// <summary>
    /// 为GameAbilityComponent准备的用于管理FGameAbilitySpec的容器
    /// </summary>
    internal class GameAbilitySpecContainer
    {
        public GameAbilitySpecContainer() { }
        public bool AddGameAbility<T>(out GameAbilitySpec retSpec) where T : GameAbility 
        {
            retSpec = GetGameAbilitySpec(typeof(T));
            if (retSpec.Handle.IsValid()) return false;
            retSpec=GameAbilitySpec.Create<T>();
            _AbilitySpecs.Add(retSpec);
            return true;
        }
       
        public GameAbilitySpec RemoveGameAbility<T>() where T : GameAbility 
        { 
            Type abilityType = typeof(T);
            return RemoveGameAbility(abilityType);
        }
     
        public GameAbilitySpec RemoveGameAbility(in Type abilityType) 
        {
            GameAbilitySpec retSpec=new GameAbilitySpec();

            if (ReferenceEquals(abilityType,null))return retSpec;
            foreach (var spec in _AbilitySpecs)
            {
                if (abilityType.Equals(spec.AbilityType))
                {
                    retSpec = spec;
                    break;
                }
            }
            return retSpec;
        }
        public GameAbilitySpec GetGameAbilitySpec<T>() where T : GameAbility 
        {
            Type abilityType = typeof(T);
           
            return GetGameAbilitySpec(abilityType);
        }

        public GameAbilitySpec GetGameAbilitySpec(in Type abilityType)
        {
            GameAbilitySpec retSpec= new GameAbilitySpec();
            foreach (var spec in _AbilitySpecs)
            {
                if (abilityType.Equals(spec.AbilityType))
                {
                    retSpec = spec;
                    break;
                }
            }
            return retSpec;
        }
        public GameAbilitySpec GetGameAbilitySpec(in GameAbilitySpecHandle specHandle)
        {
            GameAbilitySpec retSpec = new GameAbilitySpec();
            if(!specHandle.IsValid())return retSpec;
            foreach (var spec in _AbilitySpecs)
            {
                if (specHandle.Equals(spec.Handle))
                {
                    retSpec = spec;
                    break;
                }
            }
            return retSpec;
        }
        public bool HasGameAbilitySpec(in GameAbilitySpec inSpec) 
        {
            return HasGameAbilitySpec(inSpec.Handle);
        }
        public bool HasGameAbilitySpec(in GameAbilitySpecHandle inSpecHandle) 
        {
            if (_AbilitySpecs.Count<=0) return false;
            foreach (var spec in _AbilitySpecs)
            {
                if (spec.Handle.Equals(inSpecHandle)) return true;
            }
            return false;
        }
        public bool HasGameAbilitySpec(in Type abilityType)
        {
            foreach (var spec in _AbilitySpecs)
            {
                if (abilityType.Equals(spec.AbilityType))
                {
                    return true;
                }
            }
            return false;
        }

        //拥有的技能
        List<GameAbilitySpec> _AbilitySpecs=new List<GameAbilitySpec>();

    }
}
