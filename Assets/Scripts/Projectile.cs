using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public LayerMask collisionMask;
    float speed = 10;
    public float damage = 1;
    public float lifetime = 3;
    float skinWidth = .1f;

    public ParticleSystem bulletOnObstacleEffect;

    private void Start()
    {
        Destroy(gameObject, lifetime);

        Collider[] colliders = Physics.OverlapSphere(transform.position, .1f, collisionMask);
        if (colliders.Length > 0)
        {
            OnHitObject(colliders[0], transform.position);
        }
    }

    public void SetSpeed(float _speed)
    {
        speed = _speed;
    }

    // Update is called once per frame
    void Update()
    {
        float moveDistance = Time.deltaTime * speed;
        CheckCollisions(moveDistance);
        transform.Translate(Vector3.forward * moveDistance);
    }

    void CheckCollisions(float moveDistance)
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, moveDistance + skinWidth, collisionMask, QueryTriggerInteraction.Collide))
        {
            OnHitObject(hit.collider, hit.point);
        }
    }

    void OnHitObject(Collider col, Vector3 hitPoint)
    {
        if (col.CompareTag("Obstacle"))
        {
            if (bulletOnObstacleEffect != null)
            {
                Destroy(Instantiate(bulletOnObstacleEffect, hitPoint, Quaternion.FromToRotation(Vector3.forward, -transform.forward)).gameObject, bulletOnObstacleEffect.startLifetime);
            }
            Destroy(gameObject);
            return;
        }

        IDamageable damageObj = col.GetComponent<IDamageable>();
        if (damageObj != null)
        {
            damageObj.TakeHit(damage, hitPoint, transform.forward);
        }
        Destroy(gameObject);
    }
}
