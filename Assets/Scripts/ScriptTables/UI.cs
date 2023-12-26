using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UIData", menuName = "GUI/Create UIData")]
public class UI : ScriptableObject
{
    [System.Serializable]
    public class UIString
    {
        public string uiName;
        public string uiRoot;
    }

    [System.Serializable]
    public class UIPrefab
    {
        public string uiName;
        public GameObject uiObj;
    }

    public UIString[] uiString;
    public UIPrefab[] uiPrefab;

    private Dictionary<string, string> uiStringDict = new Dictionary<string, string>();
    private Dictionary<string, GameObject> uiPrefabDict = new Dictionary<string, GameObject>();

    public void Init()
    {
        uiStringDict.Clear();
        foreach (UIString i in uiString)
        {
            uiStringDict.Add(i.uiName, i.uiRoot);
        }

        uiPrefabDict.Clear();
        foreach (UIPrefab i in uiPrefab)
        {
            uiPrefabDict.Add(i.uiName, i.uiObj);
        }

        Debug.Log("UIData Init Done");
    }

    public GameObject GetUIPrefab(string name)
    {
        GameObject prefab;
        if (uiPrefabDict.TryGetValue(name, out prefab))
        {
            return prefab;
        }

        return null;
    }
}
