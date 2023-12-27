using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public Transform weaponHold;
    public Gun[] equipGuns = new Gun[3];
    Gun equippedGun;

    public System.Action<Gun> EventUnEquipGun;
    public System.Action<Gun> EventEquipGun;
    public System.Action<int> EventBulletCntChanged;

    private void Start()
    {
        if (equipGuns.Length > 0 && equipGuns[0] != null)
        {
            EquipGun(equipGuns[0]);
        }
    }

    public void ReEquipStartGun()
    {
        // TODO:
        if (equipGuns.Length > 0 && equipGuns[0] != null)
        {
            EquipGun(equipGuns[0]);
        }
    }

    public void EquipGun(int idx)
    {
        if (idx < equipGuns.Length && equipGuns[idx] != null)
        {
            EquipGun(equipGuns[idx]);
        }
    }

    public void EquipGun(Gun gunToEquip)
    {
        if (equippedGun != null){
            OnUnEquipGun();
            Destroy(equippedGun.gameObject);
        }
        equippedGun = Instantiate(gunToEquip, weaponHold.position, weaponHold.rotation);
        equippedGun.transform.parent = weaponHold;
        OnEquipGun();
    }

    public void OnTriggerHold()
    {
        if (equippedGun != null)
        {
            equippedGun.OnTriggerHold();
        }
    }

    public void OnTriggerRelease()
    {
        if (equippedGun != null)
        {
            equippedGun.OnTriggerRelease();
        }
    }

    public void Aim(Vector3 aimPoint)
    {
        if (equippedGun != null)
        {
            equippedGun.Aim(aimPoint);
        }
    }

    public void Reload()
    {
        if (equippedGun != null)
        {
            equippedGun.Reload();
        }
    }

    void OnBulletCntChanged(int cnt)
    {
        if (EventBulletCntChanged != null)
            EventBulletCntChanged(cnt);
    }

    void OnEquipGun()
    {
        if (EventEquipGun != null)
            EventEquipGun(equippedGun);
        if (equippedGun != null)
            equippedGun.EventBulletCntChanged += OnBulletCntChanged;
        OnBulletCntChanged(equippedGun.bulletCount);
    }

    void OnUnEquipGun()
    {
        if (EventUnEquipGun != null)
            EventUnEquipGun(equippedGun);
        if (equippedGun != null)
            equippedGun.EventBulletCntChanged -= OnBulletCntChanged;
    }
}
