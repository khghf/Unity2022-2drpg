using GFW.GameAbilitySystem.GameAttributeN;
using GFW.GameAbilitySystem.GameEffectN.DecoratorN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFW.GameAbilitySystem.GameEffectN
{
    public class GameEffect
    {
        public GameEffect() { }
        protected T AddDecorator<T>()where T : GameEffectDecorator,new()
        {
            T gameEffectDecorator = new T();
            _Decorators.Add(gameEffectDecorator);
            return gameEffectDecorator;
        }

        protected T AddModifier<T>() where T : AttributeModifierBase, new()
        {
            T gameEffectDecorator = new T();
            _AttributeModifiers.Add(gameEffectDecorator);
            return gameEffectDecorator;
        }

        internal bool CanApply()
        {
            foreach (var decorator in _Decorators)
            {
                if(!decorator.CanEffectApply())return false;
            }
            return true;
        }


        /// <summary>
        /// GameEffect的配置项，包含：持续时间、触发周期、是否堆叠......
        /// </summary>
        public EffectConfig Config=new EffectConfig();

        /// <summary>
        /// GameEffect上下文，存储一些经常访问的信息，包含：应用该GameEffect的GAC、该GameEffect作用于的目标......
        /// </summary>
        internal EffectContext Context=new EffectContext();
        
        protected List<GameEffectDecorator> _Decorators=new List<GameEffectDecorator>();
        public List<GameEffectDecorator> Decorators => _Decorators;

        /// <summary>
        /// 执行GameEffect时应用的属性修改器
        /// </summary>
        protected List<AttributeModifierBase> _AttributeModifiers = new List<AttributeModifierBase>();
        public List<AttributeModifierBase> AttributeModifiers => _AttributeModifiers;

        
    }
}
