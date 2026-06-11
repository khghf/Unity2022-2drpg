using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TransitionLevel : MonoBehaviour
{

    [SerializeField] Image BG;
    [SerializeField] Color BGFadeInColor = new Color(0, 0, 0, 1);
    [SerializeField] Color BGFadeOutColor = new Color(0, 0, 0, 0);
    [SerializeField] public readonly float FadeInTime = 3;
    [SerializeField] public readonly float FadeOutTime = 3;


    [SerializeField] TextMeshProUGUI Tips;
    [SerializeField] Color TipsFadeInColor = new Color(1, 1, 1, 1);
    [SerializeField] Color TipsFadeOutColor = new Color(1, 1, 1, 0);

    private void Awake()
    {
        Canvas canvas = GetComponent<Canvas>();
        if (canvas == null) return;
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 9999; 
    }
    private void Start()
    {
        BG.color=BGFadeOutColor;
        Tips.color=TipsFadeOutColor;
        PlayBGFadeInAnim();
    }
    public void PlayBGFadeInAnim()
    {
        if (BG==null) return;
        BG.DOColor(BGFadeInColor/3*2, FadeInTime/3*2)
            .OnComplete(() =>
            {
                BG.DOColor(BGFadeInColor, FadeInTime/3);
                Tips.DOColor(TipsFadeInColor, FadeInTime/3);
            });
    }
    public void PlayBGFadeOutAnim()
    {
        if (BG==null) return;
        BG.DOColor(BGFadeOutColor, FadeOutTime);
        Tips.DOColor(TipsFadeOutColor, FadeOutTime/3);
    }

}
