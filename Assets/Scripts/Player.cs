using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GunController))]
[RequireComponent(typeof(PlayerController))]
public class Player : LivingEntity
{
    public float moveSpeed = 5f;
    Camera viewCamera;
    Plane groundPlane;
    PlayerController controller;
    GunController gunController;

    bool canControllPlayer;

    protected void Awake()
    {
        controller = GetComponent<PlayerController>();
        gunController = GetComponent<GunController>();
        viewCamera = Camera.main;
        groundPlane = new Plane(Vector3.up, Vector3.zero);
    }

    protected override void Start()
    {
        base.Start();
        if (Game.Instance != null)
        {
            Game.Instance.EventNextWaveBegin += NextWaveBegin;
            Game.Instance.EventNextWaveEnd += NextWaveEnd;
        }
        else
        {
            NextWaveEnd();
        }
        if (gunController != null)
        {
            gunController.EventEquipGun += OnEquipGun;
            gunController.EventUnEquipGun += OnUnEquipGun;
        }
    }

    private void OnDestroy()
    {
        if (Game.Instance != null)
        {
            Game.Instance.EventNextWaveBegin -= NextWaveBegin;
            Game.Instance.EventNextWaveEnd -= NextWaveEnd;
        }
        if (gunController != null)
        {
            gunController.EventEquipGun -= OnEquipGun;
            gunController.EventUnEquipGun -= OnUnEquipGun;
        }
    }

    void Update()
    {
        if (!canControllPlayer)
            return;
        // Movement Input
        Vector3 direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        controller.Move(direction.normalized * moveSpeed);

        // Look At Input
        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
        float rayDistance;

        if (groundPlane.Raycast(ray, out rayDistance))
        {
            Vector3 point = ray.GetPoint(rayDistance);
            //Debug.DrawLine(ray.origin, point, Color.red)
            controller.LookAt(point);
            if ((new Vector2(point.x, point.z) - new Vector2(transform.position.x, transform.position.z)).sqrMagnitude > 1)
                gunController.Aim(point);
        }

        // Weapon Input
        if (Input.GetMouseButton(0))
        {
            gunController.OnTriggerHold();
        }

        if (Input.GetMouseButtonUp(0))
        {
            gunController.OnTriggerRelease();
        }

        if (Input.GetKey(KeyCode.R))
        {
            gunController.Reload();
        }
    }

    public GunController GunController
    {
        get
        {
            return gunController;
        }
    }

    void NextWaveBegin()
    {
        canControllPlayer = false;
    }

    void NextWaveEnd()
    {
        canControllPlayer = true;
        gunController.ReEquipStartGun();
    }

    void OnEquipGun(Gun gun)
    {

    }

    void OnUnEquipGun(Gun gun)
    {
        
    }

    [ContextMenu("Self Destroy")]
    protected override void Dead()
    {
        Game.Instance.AudioManager.PlaySound("PlayerDead", transform.position);
        OnDeath();
        gameObject.SetActive(false);
    }
}
