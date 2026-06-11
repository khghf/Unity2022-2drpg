using GFW.GameAbilitySystem.GameAttributeN;
using System;
using UnityEngine;
//[CreateAssetMenu(fileName ="CharacterAttribute",menuName ="Character/Attribute")]
[Serializable]
public class CharacterAttribute : GameAttributeSet
{
    public GameAttribute MoveSpeed =new GameAttribute();

    public GameAttribute Hp = new GameAttribute();

    public GameAttribute Atk = new GameAttribute();

    //普通攻击间隔
    public GameAttribute CommonAtkInterval = new GameAttribute();

    //x轴攻击范围
    public GameAttribute CommonAtkRangle_X = new GameAttribute();
    //y轴攻击范围
    public GameAttribute CommonAtkRangle_Y = new GameAttribute();
}
