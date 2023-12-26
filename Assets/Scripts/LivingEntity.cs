using UnityEngine;

public class LivingEntity : MonoBehaviour, IDamageable
{
    public float startingHealth;
    public bool superman;
    protected float health;
    protected bool isDead;

    public event System.Action EventOnDeath;

    protected virtual void Start()
    {
        health = startingHealth;
    }

    public virtual void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        // do something
        TakeDamage(damage);
    }

    public virtual void TakeDamage(float damage)
    {
        if (superman) return;
        health -= damage;
        if (health <= 0 && !isDead)
        {
            Dead();
        }
    }

    protected virtual void Dead()
    {
        OnDeath();
        Destroy(gameObject);
    }

    protected void OnDeath()
    {
        isDead = true;
        if (EventOnDeath != null)
        {
            EventOnDeath();
        }
    }
}
