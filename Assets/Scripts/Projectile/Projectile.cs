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

    protected virtual void FixedUpdate()
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
            return;
        }

        // Get the nearby creeps from the SpatialHashGrid
        Creep nearestCreep = WaveManager.Instance.SpatialHashGrid.GetNearestCreep(transform.position, 1f);
        if (nearestCreep != null)
        {
            float distanceToCreep = Vector3.Distance(transform.position, nearestCreep.transform.position);
            if (distanceToCreep <= 2f)
            {
                nearestCreep.TakeDamage(damage);
                ApplyEffect(nearestCreep);
                hasHitTarget = true;
                ReturnToPool();
                hasHitTarget = false;
                return;
            }
        }
    }

    public void SetDamage(float damage)
    {
        this.damage = damage;
    }

    protected abstract void ApplyEffect(Creep target);

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
