using System.Collections;
using UnityEngine;

//  https://blog.csdn.net/m0_37602827/article/details/131587207

[DisallowMultipleComponent]  // 不允许同一对象添加多个该组件
public class DissolveEffect : MonoBehaviour
{
    private Renderer[] renderers;

    [SerializeField]
    private Material dissolveMat;
    private float burnSpeed = 0.25f;
    private float burnAmount = 1;

    // Start is called before the first frame update
    void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
    }

    private void Start()
    {
        
    }

    public void OnAppear()
    {
        foreach (Renderer renderer in renderers)
        {
            Material[] materials = renderer.sharedMaterials;
            Material[] dissloveMaterials = new Material[materials.Length];
            for (int i = 0; i < materials.Length; i++)
            {
                Material newMat = new Material(dissolveMat);
                SetTexture(materials[i], newMat);
                SetColor(materials[i], newMat);
                newMat.SetFloat("_BurnAmount", 1);
                dissloveMaterials[i] = newMat;
            }
            renderer.sharedMaterials = dissloveMaterials;
        }

        StartCoroutine(Appear());
    }

    IEnumerator Appear()
    {
        while(burnAmount > 0)
        {
            yield return null;
            burnAmount -= Time.deltaTime * burnSpeed;
            foreach (Renderer renderer in renderers)
            {
                Material[] materials = renderer.sharedMaterials;
                foreach (Material material in materials)
                {
                    material.SetFloat("_BurnAmount", burnAmount);
                }
            }
        }
    }

    private void SetTexture(Material oldMat, Material newMat)
    {
        if (oldMat.HasTexture("_MainTex"))
        {
            Texture texture = oldMat.GetTexture("_MainTex");
            newMat.SetTexture("_MainTex", texture);
        }
    }

    private void SetColor(Material oldMat, Material newMat)
    {
        Color color = Color.white;
        if (oldMat.HasColor("_Color"))
        {
            color = oldMat.GetColor("_Color");
        }
        newMat.SetColor("_Color", color);
    }
}
