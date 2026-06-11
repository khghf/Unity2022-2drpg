using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFW.GameAbilitySystem.GameEffectN
{
    public class EffectContext
    {
        public EffectContext() 
        {
            Instigator=null;
            Target=null;
        }
        /// <summary>
        /// 触发该GameEffect的GameAbinityComponet,
        /// 例：B->ApplyToTarget(A),Instigator=B
        /// </summary>
        public GameAbilityComponent  Instigator;
        //// <summary>
        /// 受到该GameEffect影响的GameAbinityComponet,
        /// 例：B->ApplyToTarget(A),Target=A
        /// </summary>
        public GameAbilityComponent  Target;
    }
}
