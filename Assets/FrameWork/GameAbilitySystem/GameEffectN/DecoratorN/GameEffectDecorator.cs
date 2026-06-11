namespace GFW.GameAbilitySystem.GameEffectN.DecoratorN
{
    /// <summary>
    /// 游戏效果装饰器，为游戏效果添加额外功能
    /// </summary>
    public class GameEffectDecorator
    {
        protected EffectContext _EffectContext;

        public GameEffectDecorator() { }
        internal void Init(EffectContext effectContext)
        {
            _EffectContext=effectContext;
        }

        /// <summary>
        /// 决定GameEffect能否应用，只要任意装饰器返回false则无法应用
        /// </summary>
        /// <returns></returns>
        public bool CanEffectApply()
        {
            return true;
        }


        /// <summary>
        /// 堆叠层数改变时触发，需要是可堆叠的GameEffect
        /// </summary>
        /// <param name="oldCount">堆叠前的层数</param>
        /// <param name="newCount">堆叠后的层数</param>
        public virtual void OnEffectStackCountChanged(int oldCount,int newCount)
        {

        }
        /// <summary>
        /// 堆叠层数溢出时触发
        /// </summary>
        /// <param name="overflowCount">溢出的层数</param>
        public virtual void OnEffectStackCountOverflow(int overflowCount)
        {

        }
        /// <summary>
        /// 被添加到激活GameEffect列表中触发,必须是非即时GameEffect
        /// </summary>
        public virtual void OnEffectAdded()
        {

        }
        /// <summary>
        /// GameEffect持续时间结束时触发,必须是非即时GameEffect
        /// </summary>
        public virtual void OnEffectDurationEnd()
        {

        }

        /// <summary>
        /// 当非即时GameEffect应用
        /// </summary>
        //public virtual void OnDurationEffectFirstExcute()
        //{

        //}

        /// <summary>
        /// GameEffect周期性执行时触发,必须是非即时GameEffect
        /// </summary>
        public virtual void OnEffectPeriodExcute()
        {

        }

        /// <summary>
        /// 只有当即时GameEffect执行时才触发
        /// </summary>
        public virtual void OnInstantEffectExcute()
        {

        }

    }
}
