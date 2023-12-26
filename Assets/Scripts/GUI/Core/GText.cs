using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GText : TextMeshProUGUI
{
    public System.Action<string> OnTextChange;

    public override string text
    {
        get
        {
            return base.text;
        }

        set
        {
            base.text = value;
            if (OnTextChange != null)
            {
                OnTextChange(value);
            }
        }
    }

    public float SetTextAdjustHeight(string text)
    {
        this.text = text;
        Vector2 oldSize = rectTransform.sizeDelta;
        Vector2 newSize = GetPreferredValues();
        rectTransform.sizeDelta = new Vector2(oldSize.x, newSize.y);

        return newSize.y - oldSize.y;
    }
}
