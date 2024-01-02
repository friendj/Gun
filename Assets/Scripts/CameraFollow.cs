using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public float smoothSpeed;

    void Start()
    {
        if (target == null)
        {
            Player player = FindObjectOfType<Player>();
            if (player != null)
            {
                target = player.transform;
                player.EventOnDeath += OnTargetDeath;
            }
        }

        if (Game.Instance != null)
        {
            Game.Instance.EventNextWaveCenter += OnResetCameraPos;
        }
        OnResetCameraPos();
    }

    private void OnDestroy()
    {
        if (Game.Instance != null)
        {
            Game.Instance.EventNextWaveCenter -= OnResetCameraPos;
        }
    }

    private void FixedUpdate()
    {
        if (target == null)
            return;

        Vector3 desiredPosition = target.position + offset;
        Vector3 SmoothPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = SmoothPosition;
    }

    void OnTargetDeath()
    {
        target = null;
    }

    void OnResetCameraPos()
    {
        if (target == null)
            return;
        transform.position = target.position + offset;
    }
}
