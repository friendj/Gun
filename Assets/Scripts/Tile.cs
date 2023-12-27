using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    Renderer render;
    static Material baseMat;
    static Material createEnemyMat;

    private void Start()
    {
        render = GetComponent<Renderer>();
        if (baseMat == null )
        {
            baseMat = render.material;
        }
        if (createEnemyMat == null)
        {
            createEnemyMat = new Material(baseMat);
            createEnemyMat.color = Color.red;
        }
    }

    public void SetCreateEnemyColor()
    {
        render.material = createEnemyMat;
    }

    public void SetNormalColor()
    {
        render.material = baseMat;
    }
}
