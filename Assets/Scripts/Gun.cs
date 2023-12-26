using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public enum FireMode { Auto, Burst, Single };
    public FireMode fireMode = FireMode.Auto;

    public Transform[] projectileSpawners;
    public Projectile projectile;
    public float msBetweenShots = 100;
    public float muzzleVelocity = 35;
    float nextShootTime;

    bool triggerReleaseSinceLastShoot;
    public int burstCount;
    int shotsRemainingInBurst;

    [Header("Effects")]
    public Transform shellEjection;
    public Shell shell;
    public MuzzleFlash muzzleFlash;

    [Header("Recoil")]
    Vector3 recoilSmoothDampVelocity;
    float recoilAngle;
    float recoilRotSmoothDampVelocity;

    [Header("Music")]
    public AudioClip shootingClip;
    public AudioClip reloadClip;

    public Vector2 kickMinMax = new Vector3(.02f, .05f);
    public float recoilMoveSettleTime = .1f;
    public Vector2 recoilAngleMinMax = new Vector2(10, 20);
    public float recoilRotationSettleTime = .1f;

    [Header("Reload")]
    public int bulletCount = 30;
    [SerializeField]
    int bulletCountInMag = 30;
    bool isReloading;

    public System.Action<int> EventBulletCntChanged;

    private void Start()
    {
        muzzleFlash = GetComponent<MuzzleFlash>();
        shotsRemainingInBurst = burstCount;
        bulletCountInMag = bulletCount;
    }

    private void LateUpdate()
    {
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, Vector3.zero, ref recoilSmoothDampVelocity, recoilMoveSettleTime);
        recoilAngle = Mathf.SmoothDamp(recoilAngle, 0, ref recoilRotSmoothDampVelocity, recoilRotationSettleTime);
        transform.localEulerAngles = transform.localEulerAngles + Vector3.left * recoilAngle;
    }

    private void Shoot()
    {
        if (isReloading)
            return;
        if (Time.time > nextShootTime)
        {
            if (bulletCountInMag <= 0)
                return;
            if (fireMode == FireMode.Burst)
            {
                if (shotsRemainingInBurst == 0)
                    return;
                shotsRemainingInBurst--;
            }
            else if (fireMode == FireMode.Single)
            {
                if (!triggerReleaseSinceLastShoot)
                    return;
            }
            for (int i = 0; i < projectileSpawners.Length; i++)
            {
                if (bulletCountInMag <= 0)
                    break;
                bulletCountInMag--;
                OnBulletCntChange();
                Projectile newProjectile = Instantiate(projectile, projectileSpawners[i].position, projectileSpawners[i].rotation) as Projectile;
                newProjectile.SetSpeed(muzzleVelocity);
            }
            nextShootTime = Time.time + msBetweenShots / 1000;
            Shell newShell = Instantiate(shell, shellEjection.position, shellEjection.rotation) as Shell;
            muzzleFlash.Activate();

            transform.localPosition += Vector3.back * .05f;
            recoilAngle += Random.Range(recoilAngleMinMax.x, recoilAngleMinMax.y);
            recoilAngle = Mathf.Clamp(recoilAngle, 0, 30);

            Game.Instance.AudioManager.PlaySound(shootingClip, transform.position);
        }
    }

    public void OnTriggerHold()
    {
        Shoot();
        triggerReleaseSinceLastShoot = false;
    }

    public void OnTriggerRelease()
    {
        triggerReleaseSinceLastShoot = true;
        shotsRemainingInBurst = burstCount;
    }

    public void Aim(Vector3 aimPoint)
    {
        if (isReloading)
            return;
        transform.LookAt(new Vector3(aimPoint.x, transform.position.y, aimPoint.z)) ;
    }

    public void Reload()
    {
        if (isReloading)
            return;
        if (bulletCountInMag == bulletCount)
            return;
        StartCoroutine(AnimReload());
        Game.Instance.AudioManager.PlaySound(reloadClip, transform.position);
    }

    IEnumerator AnimReload()
    {
        isReloading = true;
        float reloadTime = 1f;
        float percent = 0;
        float upAngle = 30;
        Vector3 baseRot = transform.localEulerAngles;


        while (percent < 1)
        {
            percent += Time.deltaTime * reloadTime;

            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
            float reloadAngle = Mathf.Lerp(0, upAngle, interpolation);
            transform.localEulerAngles = baseRot + Vector3.left * reloadAngle;
            yield return null;
        }

        bulletCountInMag = bulletCount;
        isReloading = false;
        OnBulletCntChange();
    }

    void OnBulletCntChange()
    {
        if (EventBulletCntChanged != null)
            EventBulletCntChanged(bulletCountInMag);
    }
}
