using System;
using UnityEngine;

// Abstract and Object Pooling Pattern
public abstract class Projectile : MonoBehaviour
{
    protected float damage;
    private bool hasHitTarget = false;
    private const float maxDistanceThreshold = 100f; // Adjust this value as needed

    private Vector3 initialPosition;

    private void Awake()
    {
        TrackDistanceOfProjectile();
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
            //Ghost projectile!
            ReturnToPool();
            hasHitTarget = false;
            return;
        }

        // Get the nearby creeps from the SpatialHashGrid
        Creep nearestCreep = WaveManager.Instance.SpatialHashGrid.GetNearestCreep(transform.position, 1f);
        if (nearestCreep != null)
        {
            float distanceToCreep = Vector3.Distance(transform.position, nearestCreep.transform.position);
            if (distanceToCreep <= 1.5f)
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

    protected abstract void ApplyEffect(Creep target);

    private void TrackDistanceOfProjectile()
    {
        initialPosition = transform.position;
    }

    public void SetDamage(float damage)
    {
        this.damage = damage;
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
