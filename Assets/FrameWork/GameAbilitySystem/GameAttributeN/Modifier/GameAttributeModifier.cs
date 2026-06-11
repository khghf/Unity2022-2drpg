using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFW.GameAbilitySystem.GameAttributeN
{
    class ModifierSettings
    {
        public ModifierSettings() 
        {
            AttributrSetHashCode=0;
            AttributeName=string.Empty;
            ValueType=EValueType.CurValue;
            OperatorType=EOperatorType.Additive;
            Value=0f;
        }
        internal int AttributrSetHashCode;
        public string AttributeName;
        public EValueType ValueType;
        public EOperatorType OperatorType;
        public float Value;
    }

    internal class GameAttributeModifier:AttributeModifierBase
    {


        public GameAttributeModifier() 
        {
            settings=new List<ModifierSettings>();
        }

        public void AddAttributeToModify<T>(ModifierSettings modifierSettings) where T:GameAttributeSet
        {
            CaptureAttribute<T>(modifierSettings.AttributeName, ECaptureAttributeSource.Source);
            modifierSettings.AttributrSetHashCode=typeof(T).GetHashCode();
            settings.Add(modifierSettings);
        }
        public override void Excute(CapturedAttributes capturedAttributes, ModifiedResults outResult)
        {
            base.Excute(capturedAttributes, outResult);
            foreach(var setting in settings)
            {
                outResult.Add(setting.AttributrSetHashCode, setting.AttributeName, setting.Value, setting.ValueType, setting.OperatorType);
            }
        }


        List<ModifierSettings> settings;
    }
}
