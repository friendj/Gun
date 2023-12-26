using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NextWaveDlg : GUIBase
{
    public Image imgBg;

    [SerializeField]
    Color baseColor;

    private void Awake()
    {
        baseColor = imgBg.color;
    }

    public override void Refresh()
    {
        StopCoroutine("OnFadeIn");
        StartCoroutine("OnFadeIn");
    }

    public void ShowIn()
    {
        StopCoroutine("OnFadeIn");
        StopCoroutine("OnFadeOut");
        StartCoroutine("OnFadeIn");
    }

    public void ShowOut()
    {
        StopCoroutine("OnFadeIn");
        StopCoroutine("OnFadeOut");
        StartCoroutine("OnFadeOut");
    }

    protected override IEnumerator OnFadeIn()
    {
        float speed = 2f;

        Color targetColor = Color.clear;
        float percent = 0;
        Game.Instance.NextWaveBegin();
        while (percent < 1)
        {

            percent += Time.deltaTime * speed;

            imgBg.color = Color.Lerp(targetColor, baseColor, percent);

            yield return null;
        }
        Game.Instance.NextWaveCenter();
    }

    protected override IEnumerator OnFadeOut()
    {
        float speed = 2f;
        Color targetColor = Color.clear;

        imgBg.color = baseColor;

        float percent = 0;
        while(percent < 1)
        {
            percent += Time.deltaTime * speed;

            imgBg.color = Color.Lerp(baseColor, targetColor, percent);

            yield return null;
        }

        Game.Instance.NewWaveEnd();
        gameObject.SetActive(false);
    }
}
