using System;
using System.Reflection;

namespace GFW.GameAbilitySystem.GameAttributeN
{
    public class GameAttributeSet:IEquatable<GameAttributeSet>
    {
        public GameAttributeSet() 
        {
            _SetType=this.GetType();        
        }

        internal float GetGameAttributeBaseValue(FieldInfo fieldInfo)
        {
            GameAttribute gameAttribute = fieldInfo.GetValue(this) as GameAttribute;
            if (gameAttribute==null)
            {
                AbilityLog.LogError("传入的字段信息不匹配");
                return 0f;
            }
            return gameAttribute.BaseValue;
        }
        internal float GetGameAttributeCurValue(FieldInfo fieldInfo)
        {
            GameAttribute gameAttribute = fieldInfo.GetValue(this) as GameAttribute;
            if (gameAttribute==null)
            {
                AbilityLog.LogError("传入的字段信息不匹配");
                return 0f;
            }
            return gameAttribute.CurValue;
        }

        internal void SetGameAttributeBaseValue(FieldInfo fieldInfo,float value)
        {
            GameAttribute gameAttribute = fieldInfo.GetValue(this) as GameAttribute;
            if(gameAttribute==null)
            {
                AbilityLog.LogError("传入的字段信息不匹配");
                return;
            }
            gameAttribute.SetBaseValue(value);
            OnGameAttributeBaseValueChanged(gameAttribute);
        }

        internal void SetGameAttributeCurValue(FieldInfo fieldInfo, float value)
        {
            GameAttribute gameAttribute = fieldInfo.GetValue(this) as GameAttribute;
            if (gameAttribute==null)
            {
                AbilityLog.LogError("传入的字段信息不匹配");
                return;
            }
            gameAttribute.SetCurValue(value);
            OnGameAttributeCurValueChanged(gameAttribute);
        }

        protected virtual void OnGameAttributeBaseValueChanged(GameAttribute gameAttribute)
        {
            OnGameAttributeChanged(gameAttribute);
        }
        protected virtual void OnGameAttributeCurValueChanged(GameAttribute gameAttribute)
        {
            OnGameAttributeChanged(gameAttribute);

        }
        protected virtual void OnGameAttributeChanged(GameAttribute gameAttribute)
        {

        }


        public static bool operator ==(GameAttributeSet a,GameAttributeSet b)
        {
            return a.Equals(b);
        }
        public static bool operator !=(GameAttributeSet a, GameAttributeSet b)
        {
            return !a.Equals(b);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as GameAttributeSet);
        }

        public bool Equals(GameAttributeSet other)
        {
            if(ReferenceEquals(other ,null))return false;
            return this._SetType.Equals(other._SetType);
        }
        public override int GetHashCode()
        {
            return _SetType.GetHashCode();
        }

        internal Type _SetType;
    }
}
