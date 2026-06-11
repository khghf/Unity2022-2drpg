using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFW.GameAbilitySystem.NGameAbility
{
    public struct AbilityContext
    {
        public AbilityContext(GameAbilityComponent  gameAbilityCompont)
        {
            OwningGAC=gameAbilityCompont;
        }
        /// <summary>
        /// 技能所属的GameAbilityComponent
        /// </summary>
        public readonly GameAbilityComponent  OwningGAC;
    }
}
