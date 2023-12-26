using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoTipsDlg : GUIBase
{
    public GText textTips;
    public Image imgTextBg;
    RectTransform rectTransform;
    Color imgTextBgBaseColor;
    Vector3 basePos;
    float offsetY;

    protected void Awake()
    {
        if (textTips != null)
        {
            textTips.OnTextChange += OnTextChange;
        }
        offsetY = imgTextBg.rectTransform.sizeDelta.y - textTips.rectTransform.sizeDelta.y;
        imgTextBgBaseColor = imgTextBg.color;
        rectTransform = GetComponent<RectTransform>();
        basePos = rectTransform.localPosition;
    }

    void OnTextChange(string text)
    {

    }

    public void SetText(string text)
    {
        float y = textTips.SetTextAdjustHeight(text);
        imgTextBg.rectTransform.sizeDelta = new Vector2(imgTextBg.rectTransform.sizeDelta.x, textTips.rectTransform.sizeDelta.y + offsetY);
        StopCoroutine("OnFadeIn");
        StartCoroutine("OnFadeIn");
    }

    IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(3);

        Image imgBg = imgTextBg.GetComponent<Image>();
        Vector3 beginPos = basePos;
        Vector3 endPos = new Vector3(beginPos.x, beginPos.y + 50, beginPos.z);
        float speed = 3f;

        float percent = 0;
        rectTransform.position = beginPos;
        while (percent < 1)
        {
            percent += Time.deltaTime * speed;
            rectTransform.localPosition = Vector3.Lerp(beginPos, endPos, percent);
            imgBg.color = Color.Lerp(imgTextBgBaseColor, Color.clear, percent);
            yield return null;
        }

        Close();
    }

    protected override IEnumerator OnFadeIn()
    {
        Image imgBg = imgTextBg.GetComponent<Image>();
        Vector3 endPos = basePos;
        Vector3 beginPos = new Vector3(endPos.x, endPos.y - 50, endPos.z);
        float speed = 3f;

        float percent = 0;
        rectTransform.position = beginPos;
        while (percent < 1)
        {
            percent += Time.deltaTime * speed;
            rectTransform.localPosition = Vector3.Lerp(beginPos, endPos, percent);
            imgBg.color = Color.Lerp(Color.clear, imgTextBgBaseColor, percent);
            yield return null;
        }
        StopCoroutine("FadeOut");
        StartCoroutine("FadeOut");
    }
}
