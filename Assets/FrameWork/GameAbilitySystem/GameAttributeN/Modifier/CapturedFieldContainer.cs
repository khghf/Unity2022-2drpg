using GFW.GameAbilitySystem.GameEffectN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GFW.GameAbilitySystem.GameAttributeN
{
    /// <summary>
    /// 管理被捕获属性的字段信息，内部缓存了属性的FieldInfo
    /// </summary>
    internal class CapturedFieldContainer
    {

        //internal CapturedAttributes CaptureAttribute(GameEffectSpec effectSpec,GameAbilityCompont GAC)
        //{
        //    CapturedAttributes ret= new CapturedAttributes();
        //    foreach (var modifier in effectSpec._EffectPrototype.AttributeModifiers)
        //    {
        //        foreach (var captureDeclare in modifier.CaptureAttributeDeclare)
        //        {
        //            CapturedAttributeField capturedAttributeField=new CapturedAttributeField();
        //            if (!HasCapturedAttributeField(captureDeclare))
        //            {
        //                GameAttributeSet gameAttributeSet = GAC._AttributeSets[captureDeclare.GameAttributeSetHashCode];
        //                if (!GAC._AttributeSets.ContainsKey(captureDeclare.GameAttributeSetHashCode))
        //                {
        //                    AbilityLog.LogError($"捕获属性失败，不存在属性集[{gameAttributeSet._SetType.Name}]");
        //                }
        //                else
        //                {
        //                    Type attributeType = gameAttributeSet._SetType;

        //                    FieldInfo attributeField = attributeType.GetField(captureDeclare.GameAttributeName, System.Reflection.BindingFlags.Public|System.Reflection.BindingFlags.NonPublic);
        //                    if (attributeField==null)
        //                    {
        //                        AbilityLog.LogError($"捕获属性失败，不存在属性[{captureDeclare.GameAttributeName}]");
        //                        return ret;
        //                    }
        //                    capturedAttributeField = new CapturedAttributeField(captureDeclare.GameAttributeSetHashCode, attributeField);
        //                    _CapturedFields[captureDeclare.GameAttributeSetHashCode].Add(capturedAttributeField);
        //                }
        //            }
        //            else
        //            {
        //                capturedAttributeField=GetCapturedAttributeField(captureDeclare);
        //            }

        //            if(captureDeclare.CaptureAttributeSource==ECaptureAttributeSource.Source)
        //            {
        //                ret.AddAttributeToSource(captureDeclare.GameAttributeSetHashCode,capturedAttributeField);
        //            }
        //            else
        //            {
        //                ret.AddAttributeToTarget(captureDeclare.GameAttributeSetHashCode, capturedAttributeField);
        //            }
        //        }
        //    }
        //    return ret;
        //}

        internal CapturedAttributes CaptureAttribute(AttributeModifierBase modifier, GameAbilityComponent  GAC)
        {
            CapturedAttributes ret = new CapturedAttributes();
            foreach (var captureDeclare in modifier.CaptureAttributeDeclare)
            {
                CapturedAttributeField capturedAttributeField = new CapturedAttributeField();
                if (!HasCapturedAttributeField(captureDeclare))
                {
                    GameAttributeSet gameAttributeSet = GAC._AttributeSets[captureDeclare.GameAttributeSetHashCode];
                    if (!GAC._AttributeSets.ContainsKey(captureDeclare.GameAttributeSetHashCode))
                    {
                        AbilityLog.LogError($"捕获属性失败，不存在属性集[{gameAttributeSet._SetType.Name}]");
                    }
                    else
                    {
                        Type attributeType = gameAttributeSet._SetType;
                        BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public|BindingFlags.NonPublic;
                        FieldInfo attributeField = attributeType.GetField(captureDeclare.GameAttributeName, bindingFlags);
                        if (attributeField==null)
                        {
                            AbilityLog.LogError($"捕获属性失败，不存在属性[{captureDeclare.GameAttributeName}]");
                            return ret;
                        }
                        capturedAttributeField = new CapturedAttributeField(captureDeclare.GameAttributeSetHashCode, attributeField);
                       
                        List<CapturedAttributeField> capturedAttributeFields = new List<CapturedAttributeField>();
                        capturedAttributeFields.Add(capturedAttributeField);
                        _CapturedFields[captureDeclare.GameAttributeSetHashCode]=capturedAttributeFields;
                    }
                }
                else
                {
                    capturedAttributeField=GetCapturedAttributeField(captureDeclare);
                }

                if (captureDeclare.CaptureAttributeSource==ECaptureAttributeSource.Source)
                {
                    ret.AddAttributeToSource(captureDeclare.GameAttributeSetHashCode, capturedAttributeField);
                }
                else
                {
                    ret.AddAttributeToTarget(captureDeclare.GameAttributeSetHashCode, capturedAttributeField);
                }
            }
            return ret;
        }


        internal bool HasCapturedAttributeField(in CaptureAttributeDeclare captureAttributeDeclare)
        {
            if (!_CapturedFields.ContainsKey(captureAttributeDeclare.GameAttributeSetHashCode)) return false;

            List<CapturedAttributeField> fields = _CapturedFields[captureAttributeDeclare.GameAttributeSetHashCode];

            foreach (CapturedAttributeField field in fields)
            {
                if (field.FieldInfo.Name.Equals(captureAttributeDeclare.GameAttributeName)) return true;
            }

            return false;
        }

        internal CapturedAttributeField GetCapturedAttributeField(in CaptureAttributeDeclare captureAttributeDeclare)
        {
            CapturedAttributeField ret=new CapturedAttributeField();
            foreach (CapturedAttributeField captureField in _CapturedFields[captureAttributeDeclare.GameAttributeSetHashCode])
            {
                if (captureField.FieldInfo.Name!=captureAttributeDeclare.GameAttributeName) continue;
                if (captureField.OwnerSetHashCode!=captureAttributeDeclare.GameAttributeSetHashCode) continue;
                ret= captureField;
                break;
            }
            return ret;
        }
        internal FieldInfo GetCapturedAttributeFieldInfo(int gameAttributeSetHashCode,string name)
        {
            FieldInfo ret=null;
            foreach (CapturedAttributeField captureField in _CapturedFields[gameAttributeSetHashCode])
            {
                if (captureField.FieldInfo.Name!=name) continue;
                ret= captureField.FieldInfo;
                break;
            }
            return ret;
        }
        /// <summary>
        /// 所属属性集合哈希码-捕获属性字段
        /// </summary>
        Dictionary<int, List<CapturedAttributeField>> _CapturedFields = new Dictionary<int, List<CapturedAttributeField>>();

    }
}
