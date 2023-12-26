using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    public UI uiData;

    public Transform _canvas;

    Dictionary<string, GUIBase> uiCache = new Dictionary<string, GUIBase>();

    public Transform canvas
    {
        get
        {
            if (_canvas == null || _canvas.IsDestroyed())
            {
                _canvas = FindObjectOfType<Canvas>().transform;
                if (_canvas == null)
                {
                    _canvas = new GameObject("Canvas").transform;
                    Canvas canvasComp = _canvas.gameObject.AddComponent<Canvas>();
                    canvasComp.renderMode = RenderMode.ScreenSpaceOverlay;

                    CanvasScaler canvasScaler = _canvas.gameObject.AddComponent<CanvasScaler>();
                    canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                    canvasScaler.matchWidthOrHeight = 0.5f;
                }
            }
            return _canvas;
        }
        set
        {
            _canvas = value;
        }
    }

    private void OnEnable()
    {
        if (uiData)
        {
            uiData.Init();
        }
    }

    private GameObject GetPrefab(string uiName)
    {
        GameObject prefab;
        prefab = uiData.GetUIPrefab(uiName);
        if (prefab != null)
            return prefab;
        return null;
    }

    public void Show(string abName, string uiName)
    {
        GUIBase ui;
        if (uiCache.TryGetValue(uiName, out ui))
        {
            ui.gameObject.SetActive(true);
            ui.Refresh();
            return;
        }

        System.Action<GameObject> action = delegate (GameObject prefab) 
        {
            GameObject obj = Instantiate(prefab, canvas);
            ui = obj.GetComponent<GUIBase>();
            if (ui != null)
            {
                uiCache[uiName] = ui;
                ui.SetName(uiName);
                ui.Refresh();
            }
            else
            {
                Debug.LogError("ui create error");
            }
        };

        Game.Instance.AssetBundleManager.LoadAssetBundle(abName, uiName, action);

    }

    public GUIBase Show(string uiName)
    {
        GUIBase ui;
        if (uiCache.TryGetValue(uiName, out ui) && !ui.IsDestroyed())
        {
            ui.gameObject.SetActive(true);
            ui.Refresh();
            return ui;
        }

        GameObject prefab = GetPrefab(uiName);
        if (prefab != null)
        {
            GameObject obj = Instantiate(prefab, canvas);
            ui = obj.GetComponent<GUIBase>();
            if (ui != null)
            {
                uiCache[uiName] = ui;
                ui.SetName(uiName);
                ui.Refresh();
                return ui;
            }
            else
            {
                Debug.LogError("ui create error");
            }
        }
        else
        {
            Debug.LogError("ui prefab get error");
        }

        return null;
    }

    public GUIBase GetUI(string uiName)
    {
        GUIBase ui;
        if(uiCache.TryGetValue(uiName, out ui))
        {
            return ui;
        }
        return null;
    }

    public void ReleaseUI(string uiName)
    {
        GUIBase ui;
        if (uiCache.TryGetValue(uiName, out ui))
        {
            uiCache.Remove(uiName);
            Destroy(ui.gameObject);
        }
    }
}
