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
        int totalFlickTimes = 10;
        int flickTimes = 0;

        render.material = attackMat;
        while (flickTimes <= totalFlickTimes)
        {

            render.material = attackMat;
            yield return new WaitForSeconds(0.1f);
            render.material = baseMat;
            yield return new WaitForSeconds(0.1f);
            flickTimes += 1;
        }

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
                        obstacle.Destroyed(Vector3.Distance(transform.position, obstaclePos), (obstaclePos - transform.position).normalized, baseMat.color);
                    }
                }
        }
        GameObject deathParticle = Instantiate(bombEffect, pos, Quaternion.identity).gameObject;
        deathParticle.GetComponent<Renderer>().sharedMaterial = baseMat;
        Destroy(deathParticle, bombEffect.startLifetime);
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
