using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFW.GameAbilitySystem.GameEffectN
{
    public class EffectConfig
    {
        public EffectConfig() 
        {
            DurationPolicy=EDurationPolicy.Instant;
            Period=0f;
            DurationTime=0f;

            StackPolicy=EStackPolicy.None;
            StackCountLimit=0;
            StackAddRefreshPolicy=EStackRefreshPolicy.RefreshDuration;
            StackRemoveRefreshPolicy=EStackRefreshPolicy.RefreshDuration;
        }
        /// <summary>
        /// 持续策略
        /// </summary>
        public EDurationPolicy DurationPolicy;
        /// <summary>
        /// 触发周期
        /// </summary>
        public float Period;
        /// <summary>
        /// 持续时长
        /// </summary>
        public float DurationTime;

        #region 堆叠
        /// <summary>
        /// 堆叠策略
        /// </summary>
        public EStackPolicy StackPolicy;
        /// <summary>
        /// 堆叠层数上限
        /// </summary>
        public int StackCountLimit;
        /// <summary>
        /// 堆叠层数增加时的时间刷新策略
        /// </summary>
        public EStackRefreshPolicy StackAddRefreshPolicy;
        /// <summary>
        /// 堆叠层数减少时的时间刷新策略
        /// </summary>
        public EStackRefreshPolicy StackRemoveRefreshPolicy;
        #endregion
    }

    /// <summary>
    /// GameEffect持续策略
    /// </summary>
    public enum EDurationPolicy
    { 
        /// <summary>
        /// 一次性的
        /// </summary>
        Instant,    
        /// <summary>
        /// 范围时间持续
        /// </summary>
        Duration,   
        /// <summary>
        /// 永久
        /// </summary>
        Infinite    
    }

    /// <summary>
    /// GameEffect堆叠策略
    /// </summary>
    public enum EStackPolicy
    {
        /// <summary>
        /// 不启用堆叠
        /// </summary>
        None,               
        /// <summary>
        /// 单独计算不同来源的层数，例如：上限为3，A对C施加了3层buff-D,B对C施加了2层，那么总共就是5层buff-D
        /// </summary>
        AggregateBySource,
        /// <summary>
        /// 计算自身所拥有的层数，  例如：上限为3,那么自身受到的buff-D的所有来源的层数之和不能超过3层
        /// </summary>
        AggregateByTarget,  
    }
    /// <summary>
    /// GameEffect堆叠刷新策略
    /// </summary>
    public enum EStackRefreshPolicy
    {
        /// <summary>
        /// 不刷新策略
        /// </summary>
        NerveRefresh,      
        /// <summary>
        /// 刷新持续时间
        /// </summary>
        RefreshDuration,   
    }
}
