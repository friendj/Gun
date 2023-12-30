using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : LivingEntity
{
    protected enum State { Idel, Chasing, Attacking};
    protected State currentState;

    public float refreshRate = 0.25f;
    public ParticleSystem deathEffect;

    protected NavMeshAgent navMeshAgent;
    protected Transform target;
    protected LivingEntity targetEntity;

    protected float attackDis = .5f;
    protected float attackBetweenAttacks = 1f;

    protected float nextAttackTime;
    protected float myCollisionRadius;
    protected float targetCollisionRadius;
    protected float damage = 1;

    protected Renderer render;
    protected static Material baseMat;
    protected static Material attackMat;

    protected bool hasTarget = false;

    [Header("drop objects(gameObject, percent(0-1))")]
    [SerializeField]
    protected DropObject[] dropObjects;

    public static event System.Action EventEnemyDeath;

    [System.Serializable]
    public class DropObject
    {
        public GameObject gameObject;
        public float dropPercent;
    };

    private void Awake()
    {
        render = GetComponent<Renderer>();
        if (baseMat == null)
        {
            baseMat = render.material;
        }

        if (attackMat == null)
        {
            attackMat = new Material(baseMat);
            attackMat.color = Color.red;
        }
    }

    protected override void Start()
    {
        base.Start();

        currentState = State.Chasing;

        navMeshAgent = GetComponent<NavMeshAgent>();
        myCollisionRadius = GetComponent<CapsuleCollider>().radius;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            target = player.transform;
            targetEntity = player.GetComponent<LivingEntity>();
            targetEntity.EventOnDeath += OnTargetDeath;
            hasTarget = true;
            targetCollisionRadius = target.GetComponent<CapsuleCollider>().radius;
            StartCoroutine(FollowTarget());
        }
    }

    void OnTargetDeath()
    {
        hasTarget = false;
        currentState = State.Idel;
    }

    protected virtual void Update()
    {
        if (hasTarget)
        {
            if (Time.time > nextAttackTime)
            {
                float sqrDstToTarget = (transform.position - target.position).sqrMagnitude;
                if (sqrDstToTarget < Mathf.Pow(attackDis + myCollisionRadius + targetCollisionRadius, 2))
                {
                    nextAttackTime = Time.time + attackBetweenAttacks;
                    StopCoroutine("Attack");
                    StartCoroutine("Attack");
                    if (Game.Instance.AudioManager)
                        Game.Instance.AudioManager.PlaySound("EnemyAttack", transform.position);
                }
            }
        }
    }

    public void SetBaseMatColor(Color color)
    {
        baseMat.color = color;
        render.material = baseMat;
    }

    protected virtual IEnumerator Attack()
    {
        navMeshAgent.enabled = false;
        currentState = State.Attacking;
        Vector3 originalPosition = transform.position;
        Vector3 targetPosition = GetTargetPos(-attackDis / 2);

        float attackSpeed = 3f;
        float percent = 0;
        bool hasAppliedDamage = false;

        render.material = attackMat;
        while(percent < 1)
        {
            if (percent > .5f && !hasAppliedDamage)
            {
                hasAppliedDamage = true;
                targetEntity.TakeDamage(damage);
            }

            percent += Time.deltaTime * attackSpeed;
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
            transform.position = Vector3.Lerp(originalPosition, targetPosition, interpolation);
            yield return null;
        }
        render.material = baseMat;

        navMeshAgent.enabled = true;
        currentState = State.Chasing;
    }

    IEnumerator FollowTarget()
    {
        while (hasTarget)
        {
            if (isDead)
                yield break;
            if (currentState == State.Chasing)
            {
                navMeshAgent.SetDestination(GetTargetPos(attackDis / 2));
            }
            yield return new WaitForSeconds(refreshRate);
        }
    }

    protected Vector3 GetTargetPos(float dis=0f)
    {
        return target.position - (target.position - transform.position).normalized * (targetCollisionRadius + myCollisionRadius + dis);
    }

    public override void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        Debug.Log("Take hit");
        if (Game.Instance.AudioManager)
            Game.Instance.AudioManager.PlaySound("Impact", transform.position);
        if (damage >= health)
        {
            //Debug.Log("Create deathEffect, " + hitDirection + " , " + transform.forward);
            if (EventEnemyDeath != null)
                EventEnemyDeath();
            if (Game.Instance.AudioManager)
                Game.Instance.AudioManager.PlaySound("EnemyDead", transform.position);
            GameObject deathParticle = Instantiate(deathEffect, hitPoint, Quaternion.FromToRotation(Vector3.forward, hitDirection)).gameObject;
            deathParticle.GetComponent<Renderer>().sharedMaterial = baseMat;
            Destroy(deathParticle, deathEffect.startLifetime);
        }
        base.TakeHit(damage, hitPoint, hitDirection);
    }

    // drop objects
    [ContextMenu("Self Destroy")]
    protected override void Dead()
    {
        DropObjects();
        base.Dead();
    }

    void DropObjects()
    {
        if (dropObjects.Length <= 0)
            return;
        int index = Random.Range(0, dropObjects.Length);
        DropObject dropObject = dropObjects[index];
        float chance = Random.Range(0f, 1f);
        if (chance < dropObject.dropPercent)
        {
            GameObject treatment = Instantiate(dropObject.gameObject, transform.position, Quaternion.identity);
            Rigidbody rigidbody = treatment.GetComponent<Rigidbody>();
            Vector3 randomDir = new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)).normalized;
            rigidbody.AddForce(randomDir * 300);
            //Destroy(treatment, 5);  // todo: 暂时5s 后续修改
        }
    }

    // end drop objects
}
