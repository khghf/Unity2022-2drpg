using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GFW.GameAbilitySystem.GameAttributeN
{
    public class GameAttribute
    {
        public GameAttribute() { }

        public GameAttribute(float baseValue)
        {
            SetBaseValue(baseValue);
        }
        public GameAttribute(float baseValue,float curValue)
        {
            SetBaseValue(baseValue);
            SetCurValue(curValue);
        }
        public float BaseValue
        {
            get => _BaseValue;
            set => SetBaseValue(value);
        }

        public float CurValue
        {
            get => _CurValue;
            set => SetCurValue(value);
        }

        public void SetValue(float value)
        {
            BaseValue = value;
            _CurValue = value;
        }
       


        

        public void SetBaseValue(float value)
        {
            _BaseValue = value;
            OnBaseValueChanged?.Invoke(_BaseValue);
        }

        public void SetCurValue(float value)
        {
            _CurValue = value;
            OnCurValueChanged?.Invoke(_CurValue);
        }

        public void ResetToBaseValue() => SetCurValue(_BaseValue);
        //public override int GetHashCode() => HashCode.Combine(_CurValue);

        protected float _BaseValue;
        protected float _CurValue;

        public Action<float> OnBaseValueChanged;
        public Action<float> OnCurValueChanged;
    }
}
