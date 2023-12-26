using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverDlg : GUIBase
{
    public Image imgBg;

    protected void OnEnable()
    {
        StopCoroutine("OnFadeIn");
        StartCoroutine("OnFadeIn");
    }

    protected override IEnumerator OnFadeIn()
    {
        if (imgBg == null)
            yield break;

        Color baseColor = imgBg.color;
        Color beginColor = Color.clear;
        float percent = 0;
        float onFadeInTime = 0f;
        float fadeInTime = 2f;
        imgBg.color = beginColor;

        while (percent < 1)
        {
            onFadeInTime += Time.deltaTime;
            percent = onFadeInTime / fadeInTime;

            imgBg.color = Color.Lerp(beginColor, baseColor, percent);
            yield return null;
        }
        imgBg.color = baseColor;
    }
}
