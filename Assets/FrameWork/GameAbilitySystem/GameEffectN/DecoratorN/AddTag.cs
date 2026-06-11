using GFW.GameAbilitySystem.GameAbilityTagN;
namespace GFW.GameAbilitySystem.GameEffectN.DecoratorN
{
    internal class AddTag:GameEffectDecorator
    {
        public AddTag() { }
        public bool RemoveTagOnEffectEnd = true;
        public override void OnInstantEffectExcute()
        {
            base.OnInstantEffectExcute();
            if(!RemoveTagOnEffectEnd)Add_Internal();
            
        }

        public override void OnEffectAdded()
        {
            base.OnEffectAdded();
            Add_Internal();
        }
        public override void OnEffectDurationEnd()
        {
            base.OnEffectDurationEnd();
            if (RemoveTagOnEffectEnd) Remove_Internal();
        }





        private void Add_Internal()
        {
            GameAbilityComponent  GAC = _EffectContext.Target;
            if (GAC!=null)GAC.AddTag(TagToAdd);
            
        }

        private void Remove_Internal()
        {
            GameAbilityComponent  GAC = _EffectContext.Target;
            if (GAC!=null) GAC.RemoveTag(TagToAdd);
        }

        public GameAbilityTagContainer TagToAdd=new GameAbilityTagContainer();
    }
}
