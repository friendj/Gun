using UnityEditor;
using System.IO;
using UnityEngine;

public class AssetBundleHelper: NormalSingleton<AssetBundleHelper>
{
    public static string dir = "./Assets/AssetBundleBuilding";

#if UNITY_EDITOR
    [MenuItem("Assets/Build All AssetBundles")]
    static void BuildAllAssetBundle()
    {
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

#if UNITY_EDITOR_OSX
        Debug.Log("building all asset bundle, platform: osx");
        BuildPipeline.BuildAssetBundles(dir, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneOSX);
#elif UNITY_EDITOR_64
        Debug.Log("building all asset bundle, platform: 64");
        BuildPipeline.BuildAssetBundles(dir, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneWindows64);
#endif
    }
#endif
}
