using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Separate the turret logic from the projectile logic, allowing for easy extension and customization.
public class Projectile : MonoBehaviour
{
    private float damage;
    private bool hasHitTarget = false;
    private const float maxDistanceThreshold = 100f; // Adjust this value as needed

    private Vector3 initialPosition;

    private void TrackDistanceOfProjectile()
    {
        initialPosition = transform.position;
    }

    private void Update()
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
            Debug.Log("Miss shoot");
        }
    }

    public void SetDamage(float damage)
    {
        this.damage = damage;
        TrackDistanceOfProjectile();
    }

    protected virtual void ApplyEffect(Creep target)
    {
        // Default implementation
    }

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
        Type type = this.GetType();
        ObjectPool<Projectile> pool;

        if (!ObjectPoolManager.Instance.ProjectilePools.TryGetValue(type, out pool))
        {
            Debug.LogErrorFormat("No object pool found for type {0}", type);
            return;
        }

        pool.ReturnObjectToPool(this);
    }


}
