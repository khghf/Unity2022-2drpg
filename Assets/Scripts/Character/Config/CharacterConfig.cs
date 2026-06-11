using GFW.GameAbilitySystem.NGameAbility;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName ="Config/Character")]
public class CharacterConfig : ScriptableObject
{
    public List<AbilityWarp> Abilities;
}
[Serializable]
public struct AbilityWarp
{
    //public MonoScript Script;
    public Sprite Icon;
    public bool IsActiveAbility;
    public GameAbility Ability;

}
