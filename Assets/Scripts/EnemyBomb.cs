using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBomb : Enemy
{
    [SerializeField]
    bool isBombing = false;

    public ParticleSystem bombEffect;
    public LayerMask obstacleLayer;

    //protected override void Start()
    //{
    //    base.Start();
    //    StartCoroutine(Attack());
    //}

    protected override IEnumerator Attack()
    {
        navMeshAgent.enabled = false;
        currentState = State.Attacking;
        int flickerDur = 5;
        int flickTimes = 10;
        int curFlickTimes = 0;
        
        float percent = 0;

        skinMaterial.color = Color.red;
        while (curFlickTimes <= flickTimes)
        {

            percent += Time.deltaTime * flickerDur;

            float i = (-Mathf.Pow(percent, 2) + percent) * 4;

            skinMaterial.color = Color.Lerp(originalColor, Color.red, i);

            if (i <= 0)
            {
                percent = 0;
                curFlickTimes++;
            }
            yield return null;
        }
        skinMaterial.color = originalColor;

        Vector3 pos = transform.position;

        Collider[] colliders = Physics.OverlapSphere(pos, 3f, obstacleLayer);
        foreach (Collider collider in colliders)
        {
                if (collider.CompareTag("Player"))
                {
                    targetEntity.TakeDamage(damage);
                }
                else
                {
                    Obstacle obstacle = collider.GetComponent<Obstacle>();
                    if (obstacle != null)
                    {
                        Vector3 obstaclePos = obstacle.transform.position;
                        obstacle.Destroyed(Vector3.Distance(transform.position, obstaclePos), (obstaclePos - transform.position).normalized, skinMaterial.color);
                    }
                }
        }

        Destroy(Instantiate(bombEffect, pos, Quaternion.identity).gameObject, bombEffect.startLifetime);
        Dead();
    }
    protected override void Update()
    {
        if (hasTarget && !isBombing)
        {
            float sqrDstToTarget = (transform.position - target.position).sqrMagnitude;
            if (sqrDstToTarget < Mathf.Pow(attackDis + myCollisionRadius + targetCollisionRadius, 2))
            {
                nextAttackTime = Time.time + attackBetweenAttacks;
                isBombing = true;
                StopCoroutine("Attack");
                StartCoroutine("Attack");
            }
        }
    }
}
