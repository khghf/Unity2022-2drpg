using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFW.GameAbilitySystem.NGameAbility
{
    /// <summary>
    /// GameAbility的主要职责是作为技能逻辑载体，对于运行时需要的数据，比如技能等级等则由FGameAbilitySpec管理
    /// </summary>
    public class GameAbility
    {
        public virtual void OnActive() { _IsActiving=true; }
        public virtual void OnCancel() { }
        public virtual void OnInterpret() { }
        public virtual void OnEnd() { }
        protected void EndAbility() { OnEnd();_IsActiving=false; _OnEnded?.Invoke(this); }

        internal bool CanActive()
        {
            return !_IsActiving;
        }

        internal void SetContext(AbilityContext inContext)
        {
            _Context = inContext;
        }



        //存储着需要频繁访问的数据
        protected AbilityContext _Context;

        public AbilityConfig Config;

        private bool _IsActiving = false;

        public event Action<GameAbility> _OnEnded;

        public event Action<GameAbility> OnEnded
        {
            add
            {
                if (_OnEnded != null && Array.Exists(_OnEnded.GetInvocationList(), d => d.Equals(value)))
                    return;
                _OnEnded += value;
            }
            remove
            {
                _OnEnded -= value;
            }
        }
    }
}
