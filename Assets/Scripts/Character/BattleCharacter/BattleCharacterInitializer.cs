using GFW.GameAbilitySystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCharacterInitializer:MonoBehaviour
{
    public CharacterAttribute attribute = null;
    public CharacterConfig characterConfig;

    public  void Init(BattleCharacter character)
    {
        GameAbilityComponent  abilityComponent = character.AC;
        List<AbilityWarp> abilities = characterConfig.Abilities;
        character.ActiveAbilities=new List<AbilityWarp>();
        foreach (AbilityWarp ability in abilities)
        {
            Type type = ability.GetType();
            AbilityWarp abilityWarp= new AbilityWarp();

            //abilityWarp.Ability=abilityComponent.AddGameAbility(type);
            abilityWarp.IsActiveAbility=ability.IsActiveAbility;
            abilityWarp.Icon=ability.Icon;

            character.ActiveAbilities.Add(abilityWarp);
        }
        //character._Attribute=abilityComponent.AddAttribute(attribute);
    }
}
