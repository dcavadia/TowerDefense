using System;
using UnityEngine;

// Abstract and Object Pooling Pattern
public abstract class Projectile : MonoBehaviour
{
    protected float damage;
    private bool hasHitTarget = false;
    private const float maxDistanceThreshold = 100f; // Adjust this value as needed

    private Vector3 initialPosition;

    protected virtual void Awake()
    {
        TrackDistanceOfProjectile();
    }

    private void TrackDistanceOfProjectile()
    {
        initialPosition = transform.position;
    }

    protected virtual void Update()
    {
        if (hasHitTarget)
            return;

        // Calculate the distance traveled by the projectile
        float distanceTraveled = Vector3.Distance(initialPosition, transform.position);

        // Check if the projectile has exceeded the maximum distance threshold
        if (distanceTraveled > maxDistanceThreshold)
        {
            ReturnToPool();
            hasHitTarget = false;
            Debug.Log("Missed shot");
        }
    }

    public void SetDamage(float damage)
    {
        this.damage = damage;
    }

    protected abstract void ApplyEffect(Creep target);

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (hasHitTarget)
            return;

        Creep creep = other.gameObject.GetComponent<Creep>();
        if (creep != null)
        {
            creep.TakeDamage(damage);
            hasHitTarget = true;
            ReturnToPool();
            hasHitTarget = false;
        }
    }

    private void ReturnToPool()
    {
        Type type = GetType();
        ObjectPool<Projectile> pool;

        if (!ObjectPoolManager.Instance.ProjectilePools.TryGetValue(type, out pool))
        {
            Debug.LogErrorFormat("No object pool found for type {0}", type);
            return;
        }

        pool.ReturnObjectToPool(this);
    }
}
