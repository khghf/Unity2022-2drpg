using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GFW.Gameplay;
using UnityEngine.UI;
using System;
public class PlayerHUD : GameHUD
{
    //技能栏
    [SerializeField]
    private GameObject SkillColumn;
    //技能按钮预制体
    [SerializeField]
    private GameObject SkillButtonPrefab;

    class ButtonToAbility
    {
        public GameObject Button;
        public AbilityWarp Ability;
        public void SetAbility(AbilityWarp abilityWarp)
        {
            this.Ability = abilityWarp;
            Button.GetComponent<Image>().sprite=abilityWarp.Icon;
        }
    }

    //技能按钮缓存
    private List<ButtonToAbility> SkillButtons;


    public event Action<AbilityWarp> OnSkillButtonClicked;
   
    private void Awake()
    {
        SkillButtons = new List<ButtonToAbility>();
        for (int i = 0; i<4; ++i)
        {
            ButtonToAbility button = GetBindButton();
            button.Button.SetActive(false);
            SkillButtons.Add(button);
        }
    }
    public void OnControlledHeroChanged(Hero hero)
    {
        List<AbilityWarp>abilities =hero.ActiveAbilities;
        if(abilities.Count>0)
        {
            UpdateSkillButtonIcon(abilities);
        }
    }

    private void UpdateSkillButtonIcon(List<AbilityWarp> abilities)
    {
        if (abilities.Count>SkillButtons.Count)
        {
            SkillButtons.AddRange(GetBindButton(abilities.Count-SkillButtons.Count));
        }
        else
        {
            for (int i = abilities.Count-1; i<SkillButtons.Count; ++i)
            {
                SkillButtons[i].Button?.SetActive(false);
            }
        }
        for (int i = 0; i<abilities.Count; ++i)
        {
            AbilityWarp ability = abilities[i];
            SkillButtons[i].Button.SetActive(true);
            SkillButtons[i].SetAbility(ability);
        }
    }

    private List<ButtonToAbility> GetBindButton(int count)
    {
        List<ButtonToAbility> list=new List<ButtonToAbility>();
        for (int i=0;i<count;++i)
        {
            list.Add(GetBindButton());
        }
        return list;
    }
    private ButtonToAbility GetBindButton()
    {
        ButtonToAbility buttonToAbility = new ButtonToAbility();
        buttonToAbility.Button=GameObject.Instantiate(SkillButtonPrefab);
        buttonToAbility.Button.GetComponent<RectTransform>().SetParent(SkillColumn.GetComponent<RectTransform>(),false);
        buttonToAbility.Button.GetComponent<Button>().onClick.AddListener(() => { OnSkillButtonClicked?.Invoke(buttonToAbility.Ability); });
        return buttonToAbility;
    }
}
