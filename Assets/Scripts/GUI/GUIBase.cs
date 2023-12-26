using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIBase : MonoBehaviour
{
    string uiName;

    List<Color> allNodeColors = new List<Color>();

    public void SetName(string name)
    {
        uiName = name;
    }

    public virtual void Refresh() {}

    protected virtual IEnumerator OnFadeIn()
    {
        yield break;
    }

    protected virtual IEnumerator OnFadeOut()
    {
        yield break;
    }

    public virtual void Close()
    {
        gameObject.SetActive(false);
    }

    public virtual void Release()
    {
        UIManager.Instance.ReleaseUI(uiName);
    }
}
