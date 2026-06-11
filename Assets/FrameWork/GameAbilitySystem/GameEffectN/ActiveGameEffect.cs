using GFW.GameAbilitySystem.TimerMgrN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFW.GameAbilitySystem.GameEffectN
{
    /// <summary>
    /// 描述处于激活状态的GameEffect(非即时GE)
    /// </summary>
    public class ActiveGameEffect
    {
        public ActiveGameEffect() { }

        public ActiveGameEffectHandle Handle;
        internal GameEffectSpec _EffectSpec;
        // 当前GameEffect的堆叠数量
        internal int _CurStackCount;

        internal TimerHandle _DurationTimerHandle;
        public bool IsPeriod => _EffectSpec._EffectPrototype.Config.Period!=0;
    }
}
