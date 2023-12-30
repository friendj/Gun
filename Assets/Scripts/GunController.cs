using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public Transform weaponHold;
    public Gun[] equipGuns = new Gun[3];
    public Gun[] equipGunInstances = new Gun[3];
    Gun equippedGun;
    int curGunIndex;

    public System.Action<Gun> EventUnEquipGun;
    public System.Action<Gun, int> EventEquipGun;
    public System.Action<int> EventBulletCntChanged;

    private void Start()
    {
        if (equipGuns.Length > 0 && equipGuns[0] != null)
        {
            EquipGun(0);
        }
    }

    public void ReEquipStartGun()
    {
        // TODO:
        if (equipGuns.Length > 0 && equipGuns[0] != null)
        {
            EquipGun(0);
        }
    }

    public void EquipGun(int idx)
    {
        if (idx < equipGuns.Length && equipGuns[idx] != null)
        {
            if (equippedGun != null)
                OnUnEquipGun();
            if (equipGunInstances[idx] == null)
                equipGunInstances[idx] = CreateGun(equipGuns[idx]);
            equippedGun = equipGunInstances[idx];
            OnEquipGun();
        }
    }

    public Gun CreateGun(Gun gun)
    {
        Gun newGun = Instantiate(gun, weaponHold.position, weaponHold.rotation);
        newGun.transform.parent = weaponHold;
        return newGun;
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
        if (equippedGun != null)
        {
            equippedGun.gameObject.SetActive(true);
        }    
        if (EventEquipGun != null)
            EventEquipGun(equippedGun, curGunIndex);
        if (equippedGun != null)
            equippedGun.EventBulletCntChanged += OnBulletCntChanged;
        OnBulletCntChanged(equippedGun.BulletCountInMag);
    }

    void OnUnEquipGun()
    {
        if (equippedGun != null)
        {
            equippedGun.gameObject.SetActive(false);
        }
        if (EventUnEquipGun != null)
            EventUnEquipGun(equippedGun);
        if (equippedGun != null)
            equippedGun.EventBulletCntChanged -= OnBulletCntChanged;
    }
}
