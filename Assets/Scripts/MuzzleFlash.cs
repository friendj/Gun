using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleFlash : MonoBehaviour
{
    public GameObject[] flashHolders;
    public Sprite[] flashSprites;
    public SpriteRenderer[] spriteRenderers;
    public float flashTime;

    private void Start()
    {
        Deactivate();
    }

    public void Activate()
    {
        foreach (GameObject flashHolder in flashHolders)
        {
            flashHolder.SetActive(true);

            int flashIndex = Random.Range(0, flashSprites.Length);
            if (flashSprites.Length > 0)
                for (int i = 0; i < spriteRenderers.Length; i++)
                {
                    spriteRenderers[i].sprite = flashSprites[flashIndex];
                }

            Invoke("Deactivate", flashTime);
        }
    }

    public void Deactivate()
    {
        foreach (GameObject flashHolder in flashHolders)
        {
            flashHolder.SetActive(false);
        }
    }
}
