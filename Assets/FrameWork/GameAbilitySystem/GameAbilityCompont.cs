using GFW.GameAbilitySystem.GameAbilityTagN;
using GFW.GameAbilitySystem.GameAttributeN;
using GFW.GameAbilitySystem.GameEffectN;
using GFW.GameAbilitySystem.GameEffectN.DecoratorN;
using GFW.GameAbilitySystem.NGameAbility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
namespace GFW.GameAbilitySystem
{
    public class GameAbilityComponent :MonoBehaviour
    {
        public GameAbilityComponent ()
        {
            _ActivableAbilitiesLock = new ReaderWriterLock();
            _ActivableAbilities = new GameAbilitySpecContainer();

            _ActiveEffectsLock = new ReaderWriterLock();
            _ActiveEffects = new ActiveGameEffectContainer(this);
    
            _OwnedTagsLock = new ReaderWriterLock();
            _OwnedTags = new GameAbilityTagContainer();

            _AttributeSetsLock = new ReaderWriterLock();
            _AttributeSets = new Dictionary<int, GameAttributeSet>();

            Owner=gameObject;
        }
        public GameAbilityComponent (object owner):this()
        {
            Owner= owner;
        }

        #region 技能激活
        public bool ActiveGameAbility<T>()where T:GameAbility
        {
            return ActiveGameAbility(typeof(T));
        }

        public bool ActiveGameAbility(in GameAbilitySpecHandle specHandle)
        {
            bool ret = false;
            _ActivableAbilitiesLock.AcquireWriterLock(100);
            GameAbilitySpec spec=_ActivableAbilities.GetGameAbilitySpec(specHandle);
            ret=ActiveGameAbility_Internal(spec);
            _ActivableAbilitiesLock.ReleaseWriterLock();
            return ret;
        }

        public bool ActiveGameAbility(Type abilityType) 
        {
            bool ret = false;
            _ActivableAbilitiesLock.AcquireWriterLock(100);
            GameAbilitySpec spec = _ActivableAbilities.GetGameAbilitySpec(abilityType);
            ret=ActiveGameAbility_Internal(spec);
            _ActivableAbilitiesLock.ReleaseWriterLock();
            return ret;
        }

        private bool ActiveGameAbility_Internal(GameAbilitySpec spec)
        {
            bool ret = false;
            if (!spec.Handle.IsValid())
            {
                AbilityLog.LogWarning($"激活技能失败，传入的类型不是GameAbility的派生类或者GAC中不存在该技能");
                return ret;
            }

            GameAbility abilityToActive = spec._AbilityPrototype;
            if (abilityToActive==null)
            {
                AbilityLog.LogWarning($"引用异常无法激活技能");
                return ret;
            }

            abilityToActive.OnEnded+=OnGameAbilityEnded;

            if (abilityToActive.CanActive())
            {
                OnGameAbilityActive(spec);
                if(abilityToActive.Config.AbilityInstantiationPolicy==EAbilityInstantiationPolicy.InstacnePreExcute)
                {
                    (Activator.CreateInstance(spec.AbilityType)as GameAbility).OnActive();
                }
                else
                {
                    abilityToActive.OnActive();
                }
            }
            return ret;
        }
        #endregion
        #region 增删 GameAbility

        public GameAbilitySpecHandle AddGameAbility<T>() where T : GameAbility 
        {
            _ActivableAbilitiesLock.AcquireWriterLock(100);
            GameAbilitySpec spec;
            if(_ActivableAbilities.AddGameAbility<T>(out spec))
            {
                AbilityContext abilityContext = new AbilityContext(this);
                spec._AbilityPrototype.SetContext(abilityContext);
                OnGameAbilityAdded(spec);
            }
            _ActivableAbilitiesLock.ReleaseWriterLock();
            return spec.Handle;
        }

        public void RemoveGameAbility<T>() where T : GameAbility 
        {
            _ActivableAbilitiesLock.AcquireWriterLock(100);
            GameAbilitySpec spec = _ActivableAbilities.RemoveGameAbility<T>();
            if (spec.Handle.IsValid()) OnGameAbilityRemoved(spec);
            _ActivableAbilitiesLock.ReleaseWriterLock();
        }
        public void RemoveGameAbility(Type abilityType)
        {
            _ActivableAbilitiesLock.AcquireWriterLock(100);
            GameAbilitySpec spec = _ActivableAbilities.RemoveGameAbility(abilityType);
            if (spec.Handle.IsValid()) OnGameAbilityRemoved(spec);
            _ActivableAbilitiesLock.ReleaseWriterLock();
        }

        public GameAbilitySpec GetGameAbilitySpec<T>() where T : GameAbility
        {
            _ActivableAbilitiesLock.AcquireReaderLock(100);
            GameAbilitySpec ret = _ActivableAbilities.GetGameAbilitySpec<T>();
            _ActivableAbilitiesLock.ReleaseReaderLock();
            return ret;
        }

        public GameAbilitySpec GetGameAbilitySpec(in Type abilityType)  
        {
            _ActivableAbilitiesLock.AcquireReaderLock(100);
            GameAbilitySpec ret = _ActivableAbilities.GetGameAbilitySpec(abilityType);
            _ActivableAbilitiesLock.ReleaseReaderLock();
            return ret;
        }

        public bool HasGameAbilitySpec(GameAbilitySpec spec)
        {
            _ActivableAbilitiesLock.AcquireReaderLock(100);
            bool ret = _ActivableAbilities.HasGameAbilitySpec(spec);
            _ActivableAbilitiesLock.ReleaseReaderLock();
            return ret;
        }
        public bool HasGameAbilitySpec(GameAbilitySpecHandle handle)
        {
            _ActivableAbilitiesLock.AcquireReaderLock(100);
            bool ret= _ActivableAbilities.HasGameAbilitySpec(handle);
            _ActivableAbilitiesLock.ReleaseReaderLock();
            return ret;
        }
        #endregion
        #region GameAbility事件通知及回调
        
        /// <summary>
        /// GameAbility被添加至容器后调用
        /// </summary>
        /// <param name="abilitySpec"></param>
        protected virtual void OnGameAbilityAdded(in GameAbilitySpec abilitySpec) 
        {
            AbilityLog.LogInfo($"GameAbility[{abilitySpec.AbilityType.Name}]:Added");


        }
        /// <summary>
        /// GameAbility被移除后调用
        /// </summary>
        /// <param name="abilitySpec"></param>
        protected virtual void OnGameAbilityRemoved(in GameAbilitySpec abilitySpec) 
        {
            AbilityLog.LogInfo($"GameAbility[{abilitySpec.AbilityType.Name}]:Removed");
        }
        /// <summary>
        /// 在开始执行GameAbility的逻辑(OnActive之前)前调用
        /// </summary>
        /// <param name="abilitySpec"></param>
        protected virtual void OnGameAbilityActive(in GameAbilitySpec abilitySpec) 
        {
            AbilityLog.LogInfo($"GameAbility[{abilitySpec.AbilityType.Name}]:Actived");
        }

        protected virtual void OnGameAbilityEnded(GameAbility gameAbility)
        {
            OnGameAbilityEnded(GetGameAbilitySpec(gameAbility.GetType()));
        }
        /// <summary>
        /// 在结束GameAbility后(OnEnd之后)调用
        /// </summary>
        /// <param name="abilitySpec"></param>
        protected virtual void OnGameAbilityEnded(in GameAbilitySpec abilitySpec)
        {
            AbilityLog.LogInfo($"GameAbility[{abilitySpec.AbilityType.Name}]:Ended");
        }
        #endregion

        #region 应用GameEffect

        public bool ApplyGameEffectToTarget<T>(GameAbilityComponent  target)where T:GameEffect,new()
        {
            _ActiveEffectsLock.AcquireWriterLock(100);

            bool ret = false;
            if (target==null)
            {
                AbilityLog.LogError("应用GameEffect失败，目标为空引用");
                return ret;
            }
            GameEffectSpec gameEffectSpec = MakeGameEffectSpec<T>();
            gameEffectSpec._EffectPrototype.Context.Target = target;

            InitGameEffectDecoractor(gameEffectSpec);

            ActiveGameEffect activeGameEffect=_ActiveEffects.ExcuteGameEffect(gameEffectSpec);

            if(activeGameEffect.Handle.IsValid())
            {
                ret=true;

                target.OnGameEffectAppliedToSelf(activeGameEffect);
                this.OnGameEffectAppliedToTarget(activeGameEffect);
            }

            _ActiveEffectsLock.ReleaseWriterLock();
            return ret;
        }

        public bool ApplyGameEffectToSelf<T>() where T : GameEffect,new() 
        {
            _ActiveEffectsLock.AcquireWriterLock(100);

            bool ret = false;
            GameEffectSpec gameEffectSpec = MakeGameEffectSpec<T>();
            gameEffectSpec._EffectPrototype.Context.Target = this;

            InitGameEffectDecoractor(gameEffectSpec);

            ActiveGameEffect activeGameEffect = _ActiveEffects.ExcuteGameEffect(gameEffectSpec);

            if (activeGameEffect.Handle.IsValid())
            {
                ret=true;
                this.OnGameEffectAppliedToSelf(activeGameEffect);
            }

            _ActiveEffectsLock.ReleaseWriterLock();
            return ret;
        }


       


        #endregion
        #region GameEffect事件通知及回调
        /// <summary>
        /// 当成功将GameEffect应用(不是已经触发了一次)于目标后触发
        /// </summary>
        /// <param name="target"></param>
        /// <param name="activeSpecHandle"></param>
        protected virtual void OnGameEffectAppliedToTarget(ActiveGameEffect activeSpecHandle)
        {
            //AbilityLog.LogInfo($"GameAbility[{abilitySpec.AbilityType.Name}]:Ended");

        }
        /// <summary>
        /// 当成功将GameEffect应用(不是已经触发了一次)于自身后触发
        /// </summary>
        /// <param name="target"></param>
        /// <param name="activeSpecHandle"></param>
        protected virtual void OnGameEffectAppliedToSelf(ActiveGameEffect activeSpecHandle)
        {

        }

        #endregion
        #region GameEffect工具函数

        internal GameEffectSpec MakeGameEffectSpec<T>() where T : GameEffect, new()
        {
            GameEffectSpec gameEffectSpec = new GameEffectSpec();
            gameEffectSpec._EffectType = typeof(T);
            gameEffectSpec._EffectPrototype=new T();
            gameEffectSpec._EffectPrototype.Context=MakeGameEffectContext();
            return gameEffectSpec;
        }
        public EffectContext MakeGameEffectContext()
        {
            EffectContext context = new EffectContext();
            context.Instigator=this;
            return context;
        }
        /// <summary>
        /// 初始化游戏效果装饰器
        /// </summary>
        /// <param name="gameEffectSpec"></param>
        private void InitGameEffectDecoractor(GameEffectSpec gameEffectSpec)
        {
            List<GameEffectDecorator> effectDecorators = gameEffectSpec._EffectPrototype.Decorators;
            foreach (GameEffectDecorator decorator in effectDecorators)
            {
                decorator.Init(gameEffectSpec._EffectPrototype.Context);
            }
        }
        #endregion

        #region 增、删、查标签

        internal void AddTag(GameAbilityTagContainer container)
        {
            if (container==null) return;
            var tags = container._Tags.ToArray();

            _OwnedTagsLock.AcquireWriterLock(100);
            foreach (var tag in tags)
            {
                AddTag(tag);
            }
            _OwnedTagsLock.ReleaseWriterLock();
        }
        internal bool AddTag(string tag)
        {
            _OwnedTagsLock.AcquireWriterLock(100);
            bool ret = false;
            GameAbilityTag newTag = GameAbilityTag.Create(tag);
            ret=AddTag(newTag);
            _OwnedTagsLock.ReleaseWriterLock();
            return ret;
        }
        internal bool AddTag(in GameAbilityTag tag)
        {
            _OwnedTagsLock.AcquireWriterLock(100);
            bool ret = _OwnedTags.AddTag(tag);
            if (ret)OnTagAdded(tag);
            _OwnedTagsLock.ReleaseWriterLock();
            return ret;
        }
        internal void RemoveTag(GameAbilityTagContainer container)
        {
            if (container==null) return;
            var tags = container._Tags.ToArray();

            _OwnedTagsLock.AcquireWriterLock(100);
            foreach (var tag in tags)
            {
                RemoveTag(tag);
            }
            _OwnedTagsLock.ReleaseWriterLock();
        }
        internal bool RemoveTag(string tag)
        {
            _OwnedTagsLock.AcquireWriterLock(100);
            GameAbilityTag newTag = GameAbilityTag.Create(tag);
            bool ret = false;
            ret=RemoveTag(newTag);
            _OwnedTagsLock.ReleaseWriterLock();
            return ret;
        }
        internal bool RemoveTag(in GameAbilityTag tag)
        {
            _OwnedTagsLock.AcquireWriterLock(100);
            bool ret = _OwnedTags.RemoveTag(tag);
            if (ret) OnTagRemoved(tag);
            _OwnedTagsLock.ReleaseWriterLock();
            return ret;
        }

        /// <summary>
        /// 检索自身的标签中(包含父级标签)是否存在指定标签
        /// </summary>
        /// <param name="tag"></param>
        /// <see cref=GameAbilityTagContainer.HasTag></ref>
        /// <returns></returns>
        public bool HasTag(GameAbilityTag tag)
        {
            _OwnedTagsLock.AcquireReaderLock(100);
            bool ret = false;
            ret=_OwnedTags.HasTag(tag);
            _OwnedTagsLock.ReleaseReaderLock();
            return ret;
        }

        /// <summary>
        /// 检索自身的标签中(不包含父级标签)是否存在指定标签
        /// </summary>
        /// <param name="tag"></param>
        /// <see cref=GameAbilityTagContainer.HasTagExact></ref>
        /// <returns></returns>
        public bool HasTagExact(GameAbilityTag tag)
        {
            _OwnedTagsLock.AcquireReaderLock(100);
            bool ret = false;
            ret=_OwnedTags.HasTagExact(tag);
            _OwnedTagsLock.ReleaseReaderLock();
            return ret;
        }
        /// <summary>
        /// 检索自身的标签中(包含父级标签)是否与tags的标签(不包含父级标签)有交集
        /// </summary>
        /// <param name="tags"></param>
        /// <see cref=GameAbilityTagContainer.HasTagAny></ref>
        /// <returns></returns>
        public bool HasTagAny(GameAbilityTagContainer tags)
        {
            _OwnedTagsLock.AcquireReaderLock(100);
            bool ret = false;
            ret=_OwnedTags.HasTagAny(tags);
            _OwnedTagsLock.ReleaseReaderLock();
            return ret;
        }
        /// <summary>
        /// 检索自身的标签中(不包含父级标签)是否与tags的标签(不包含父级标签)有交集
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public bool HasTagAnyExcat(GameAbilityTagContainer tags)
        {
            _OwnedTagsLock.AcquireReaderLock(100);
            bool ret = false;
            ret=_OwnedTags.HasTagAnyExcat(tags);
            _OwnedTagsLock.ReleaseReaderLock();
            return ret;
        }

        #endregion
        #region 标签事件通知及回调
        private void OnTagAdded(in GameAbilityTag tag)
        {
            AbilityLog.LogInfo($"OnTagAdded:Tag[{tag.ToString()}]");
        }
        private void OnTagRemoved(in GameAbilityTag tag)
        {
            AbilityLog.LogInfo($"OnTagRemoved:Tag[{tag.ToString()}]");

        }
        #endregion

        public void AddGameAttributeSet<T>()where T : GameAttributeSet,new()
        {
            _AttributeSetsLock.AcquireWriterLock(100);
            Type type = typeof(T);
            int hashCode=type.GetHashCode();
            if (!_AttributeSets.ContainsKey(hashCode))
            {
                _AttributeSets.Add(type.GetHashCode(),new T());
            }
            _AttributeSetsLock.ReleaseWriterLock();
        }


        public readonly object Owner = null;

        private readonly ReaderWriterLock _ActivableAbilitiesLock ;
        internal GameAbilitySpecContainer _ActivableAbilities ;

        private readonly ReaderWriterLock _ActiveEffectsLock ;
        internal ActiveGameEffectContainer _ActiveEffects;

        private readonly ReaderWriterLock _OwnedTagsLock;
        internal GameAbilityTagContainer _OwnedTags ;

        private readonly ReaderWriterLock _AttributeSetsLock;
        internal Dictionary<int, GameAttributeSet> _AttributeSets;
    }
}
