using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

public class AssetBundleManager : Singleton<AssetBundleManager>
{
    private string assetBundleUrl = "http://127.0.0.1:8080/Assets/AssetBundleBuilding/{0}";

    private Dictionary<string, AssetBundle> assetBundles = new Dictionary<string, AssetBundle>();

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    protected override void OnDestroy()
    {
        UnloadAll();
    }

    public void UnloadAll()
    {
        foreach (var assetBundle in assetBundles)
        {
            AssetBundle ab = assetBundle.Value;
            if (ab != null && !ab.IsDestroyed())
            {
                ab.Unload(false);
            }
        }
        assetBundles.Clear();
    }

    public void LoadAssetBundle(string name, string objName=null, System.Action<GameObject> callback=null)
    {
        if (!assetBundles.ContainsKey(name))
        {
            string path = Application.streamingAssetsPath + "/" + name;
            if (File.Exists(path))
            {
                AssetBundle assetBundle = AssetBundle.LoadFromFile(path);
                assetBundles[name] = assetBundle;
                RealLoadAssetBundle(name, objName, callback);
            }
            else
            {
                StartCoroutine(DownLoadAssetBundle(name, objName, callback));
            }
        }
        else
        {
            RealLoadAssetBundle(name, objName, callback);
        }
    }

    void RealLoadAssetBundle(string name, string objName, System.Action<GameObject> callback = null)
    {
        AssetBundle assetBundle = assetBundles[name];
        if (objName != null && callback != null)
        {
            GameObject obj = assetBundle.LoadAsset<GameObject>(objName);
            if (obj != null)
            {
                callback(obj);
            }
        }
    }

    IEnumerator DownLoadAssetBundle(string name, string objName=null, System.Action<GameObject> callback=null)
    {
        string url = string.Format(assetBundleUrl, name);
        UnityWebRequest request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();

        if (request.isDone && request.result == UnityWebRequest.Result.Success)
        {
            string savePath = Application.streamingAssetsPath + "/" + name;
            if (!Directory.Exists(Path.GetDirectoryName(savePath)))
                Directory.CreateDirectory(Path.GetDirectoryName(savePath));
            FileStream fs = File.Create(savePath);
            fs.Write(request.downloadHandler.data, 0, request.downloadHandler.data.Length);
            fs.Flush();
            fs.Close();
            fs.Dispose();
            LoadAssetBundle(name, objName, callback);
        }
        else
        {
            Debug.LogError(string.Format("Download asset bundle Error, error:{0}", request.error));
        }
    }
}
