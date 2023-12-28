using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CapPicture : MonoBehaviour
{
    public Camera capCamera;
    RenderTexture renderTexture;
    Texture2D texture;

    private void Start()
    {
        if (capCamera == null)
            return;
        renderTexture = new RenderTexture(800, 600, 32);
        texture = new Texture2D(800, 600, TextureFormat.ARGB32, false);
        capCamera.targetTexture = renderTexture;
    }

    void Update()
    {
        if (capCamera == null)
            return;
        if (Input.GetKeyUp(KeyCode.Space))
        {
            RenderTexture.active = renderTexture;
            texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            texture.Apply();
            RenderTexture.active = null;

            byte[] bytes = texture.EncodeToPNG();
            File.WriteAllBytes(Application.dataPath + "//Textures//CapPic//" + (int)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds + ".png", bytes);
        }
    }
}
