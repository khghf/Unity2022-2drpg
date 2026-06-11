using GFW.GameAbilitySystem.GameAttributeN;
using GFW.GameAbilitySystem.GameEffectN.DecoratorN;
using System;
using System.Collections.Generic;
using System.Reflection;
namespace GFW.GameAbilitySystem.GameEffectN
{
    /// <summary>
    /// 协助GameAbilityComponent管理GameEffect的执行
    /// </summary>
    public class ActiveGameEffectContainer
    {

        public ActiveGameEffectContainer(GameAbilityComponent  owner) { _Owner=owner; }
        

        internal ActiveGameEffect ExcuteGameEffect(GameEffectSpec effectSpec)
        {
            ActiveGameEffect ret = GetActiveGameEffect(effectSpec);

            if (!effectSpec._EffectPrototype.CanApply())
            {
                return new ActiveGameEffect();
            }


            if (!ret.Handle.IsValid())
            {
                ret._EffectSpec = effectSpec;
                ProcessNewGameEffect(ret);
            }
            else
            {
                //句柄有效说明存在已激活的相同GameEffect
                ProcessGameEffectStack(ret);
            }


            return ret;
        }

        /// <summary>
        /// 处理新应用的GameEffect
        /// </summary>
        /// <param name="activeGameEffect"></param>
        private void ProcessNewGameEffect(ActiveGameEffect activeGameEffect)
        {
            ActiveGameEffectHandle newHandle = ActiveGameEffectHandle.Create();
            activeGameEffect.Handle=newHandle;
            ++activeGameEffect._CurStackCount;
            GameEffectSpec effectSpec = activeGameEffect._EffectSpec;

            GameEffect gameEffect = effectSpec._EffectPrototype;

            if (gameEffect.Config.DurationPolicy==EDurationPolicy.Instant)
            {
                ForeachDecorator(activeGameEffect._EffectSpec, decorator => { decorator?.OnInstantEffectExcute(); });
                ApplyAttributeModifier(activeGameEffect._EffectSpec);
            }
            else
            {
                if (gameEffect.Config.DurationPolicy==EDurationPolicy.Infinite)
                {
                    activeGameEffect._DurationTimerHandle=TimerMgrN.TimerMgr.Inst.CreateTimer(-1f, gameEffect.Config.Period);
                    activeGameEffect._DurationTimerHandle.SetDelayEndCallback(() => { OnActiveGameEffectDurationEnd(activeGameEffect); });
                }
                else
                {
                    activeGameEffect._DurationTimerHandle=TimerMgrN.TimerMgr.Inst.CreateTimer(gameEffect.Config.DurationTime, gameEffect.Config.Period);
                }
                activeGameEffect._DurationTimerHandle.SetPeriodCallback(() => { OnActiveGameEffectPeriodExcute(activeGameEffect); });
                _ActiveEffects.Add(activeGameEffect);
                OnActiveGameEffectAdded(activeGameEffect);
            }
        }

        /// <summary>
        /// 处理GameEffect的堆叠情况
        /// </summary>
        /// <param name="activeGameEffect"></param>
        private void ProcessGameEffectStack(ActiveGameEffect activeGameEffect)
        {
            GameEffect effect = activeGameEffect._EffectSpec._EffectPrototype;
            EffectConfig effectConfig=activeGameEffect._EffectSpec._EffectPrototype.Config;
            if (effectConfig.StackPolicy==EStackPolicy.None)
            {
                return;
            }
            else if (effectConfig.StackPolicy==EStackPolicy.AggregateBySource)
            {
                //@todo
            }
            else
            {
                if(activeGameEffect._CurStackCount>=effect.Config.StackCountLimit)
                {
                    OnStackCountOverflow(activeGameEffect,1);
                }
                else
                {
                    OnStackCountChanged(activeGameEffect, activeGameEffect._CurStackCount++);
                }
            }
        }

        private void ApplyAttributeModifier(GameEffectSpec gameEffectSpec)
        {
            foreach (var modifier in gameEffectSpec._EffectPrototype.AttributeModifiers)
            {
                CapturedAttributes capturedAttributes = _CapturedAttribute.CaptureAttribute(modifier, _Owner);

                ModifiedResults modifiedResults = new ModifiedResults();
                modifier.Excute(capturedAttributes,modifiedResults);
                Modify_Internal(modifiedResults);
            }
        }

        private void UndoModifier(GameEffectSpec gameEffectSpec)
        {
            foreach (var modifier in gameEffectSpec._EffectPrototype.AttributeModifiers)
            {
                CapturedAttributes capturedAttributes = _CapturedAttribute.CaptureAttribute(modifier, _Owner);
                Modify_Internal(modifier.ModifiedResults,true);
            }
        }


        private void Modify_Internal(ModifiedResults modifiedResults,bool undo=false)
        {
            foreach(var result in modifiedResults._Results)
            {
                GameAttributeSet gameAttributeSet = _Owner._AttributeSets[result.AttributeSetHashCode];
                FieldInfo modifiedAttributeFieldInfo = _CapturedAttribute.GetCapturedAttributeFieldInfo(result.AttributeSetHashCode, result.GameAttributeName);
                if (result.ValueType==EValueType.BaseValue)
                {
                    float finalValue = gameAttributeSet.GetGameAttributeBaseValue(modifiedAttributeFieldInfo);
                    switch (result.OperatorType)
                    {
                        case EOperatorType.Additive:
                            if(undo) finalValue-=result.Value;
                            else finalValue+=result.Value;
                            break;
                        case EOperatorType.Multi:
                            if (undo) finalValue/=result.Value;
                            else finalValue*=result.Value;
                            break;
                        case EOperatorType.Divide:
                            if (undo) finalValue*=result.Value;
                            else finalValue/=result.Value;
                            break;
                        case EOperatorType.Override:
                            finalValue=result.Value;
                            break;
                    }
                    gameAttributeSet.SetGameAttributeBaseValue (modifiedAttributeFieldInfo, finalValue);
                }
                else
                {
                    float finalValue = gameAttributeSet.GetGameAttributeCurValue(modifiedAttributeFieldInfo);
                    switch (result.OperatorType)
                    {
                        case EOperatorType.Additive:
                            if (undo) finalValue-=result.Value;
                            else finalValue+=result.Value;
                            break;
                        case EOperatorType.Multi:
                            if (undo) finalValue/=result.Value;
                            else finalValue*=result.Value;
                            break;
                        case EOperatorType.Divide:
                            if (undo) finalValue*=result.Value;
                            else finalValue/=result.Value;
                            break;
                        case EOperatorType.Override:
                            finalValue=result.Value;
                            break;
                    }
                    gameAttributeSet.SetGameAttributeCurValue (modifiedAttributeFieldInfo, finalValue);
                }
            }
        }




        #region 事件回调
        private void OnStackCountChanged(ActiveGameEffect activeGameEffect,int oldCount)
        {
            AbilityLog.LogInfo($"OnStackCountChanged:oldCount[{oldCount}],newCount[{activeGameEffect._CurStackCount}]");
            ForeachDecorator(activeGameEffect._EffectSpec, decorator => { decorator?.OnEffectStackCountChanged(oldCount, activeGameEffect._CurStackCount); });
           
        }

        private void OnStackCountOverflow(ActiveGameEffect activeGameEffect,int overflowCount)
        {
            AbilityLog.LogInfo($"OnStackCountOverflow:countLimit[{activeGameEffect._EffectSpec._EffectPrototype.Config.StackCountLimit}],overflowCount[{overflowCount}]");
            ForeachDecorator(activeGameEffect._EffectSpec, decorator => { decorator?.OnEffectStackCountOverflow(overflowCount); });
           
        }

        private void OnActiveGameEffectAdded(ActiveGameEffect activeGameEffect)
        {
            AbilityLog.LogInfo($"OnActiveGameEffectAdded:name[{activeGameEffect._EffectSpec._EffectType.Name}]");
            ForeachDecorator(activeGameEffect._EffectSpec, decorator => { decorator?.OnEffectAdded(); });

         
        }

        private void OnActiveGameEffectDurationEnd(ActiveGameEffect activeGameEffect)
        {
            AbilityLog.LogInfo($"OnActiveGameEffectDurationEnd:name[{activeGameEffect._EffectSpec._EffectType.Name}]");

            ForeachDecorator(activeGameEffect._EffectSpec, decorator => { decorator?.OnEffectDurationEnd();});

            if (!activeGameEffect.IsPeriod) UndoModifier(activeGameEffect._EffectSpec);
        }

        private void OnActiveGameEffectPeriodExcute(ActiveGameEffect activeGameEffect)
        {
            AbilityLog.LogInfo($"OnActiveGameEffectPeriodExcute:name[{activeGameEffect._EffectSpec._EffectType.Name}]");
            ForeachDecorator(activeGameEffect._EffectSpec, decorator => { decorator?.OnEffectPeriodExcute();});
            ApplyAttributeModifier(activeGameEffect._EffectSpec);
        }
        #endregion

        #region 工具函数
        internal ActiveGameEffect GetActiveGameEffect(GameEffectSpec effectSpec)
        {
            ActiveGameEffect ret=new ActiveGameEffect();
            foreach(var activeGameEffect in _ActiveEffects)
            {
                if(activeGameEffect._EffectSpec.Equals(effectSpec))
                {
                    ret = activeGameEffect;
                    break;
                }
            }
            return ret;
        }

        internal static void ForeachDecorator(GameEffectSpec effectSpec,Action<GameEffectDecorator> action)
        {
            List<GameEffectDecorator> decorators = effectSpec._EffectPrototype.Decorators;
            foreach (GameEffectDecorator decorator in decorators)
            {
                action?.Invoke(decorator);
            }
        }

        //internal void ForeachModifier(GameEffectSpec effectSpec, Action<AttributeModifier> action)
        //{
        //    List<AttributeModifier> modifiers = effectSpec._EffectPrototype.AttributeModifiers;
        //    foreach (AttributeModifier modifier in modifiers)
        //    {
        //        action?.Invoke(modifier);
        //    }
        //}

        #endregion

       

        GameAbilityComponent  _Owner = null;

        //激活的GameEffect,注意：只包含非即时GameEffect
        List<ActiveGameEffect> _ActiveEffects = new List<ActiveGameEffect>();

        CapturedFieldContainer _CapturedAttribute=new CapturedFieldContainer();

    }
}
