using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretFreezer : Turret 
{
    private float fireCountdown = 0f;
    private float projectileSpeed = 50f;

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
    }

    protected override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);
    }

    // By aiming slightly ahead of the target's movement, you can increase the chance of hitting it,
    // especially if it's moving in a predictable pattern. TurretRegular and TurretFreezer apply this.
    public override void Shoot()
    {
        if (target == null)
            return;

        if (fireCountdown <= 0f)
        {
            base.CheckNearestTarget();

            // Check if the target is still valid
            if (target == null)
                return;

            // Calculate the distance between the turret and the target
            float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);

            // Calculate the time it will take for the projectile to reach the target
            float timeToTarget = distanceToTarget / projectileSpeed;

            // Calculate the estimated position of the target at the time the projectile reaches it
            Vector3 estimatedTargetPosition = target.transform.position + (target.GetComponent<Rigidbody>().velocity * timeToTarget);

            // Instantiate and shoot the projectile
            Projectile projectile = SpawnProjectile();

            projectile.SetDamage(turretData.Projectile.Damage);
            Rigidbody rb = projectile.gameObject.GetComponent<Rigidbody>();

            rb.velocity = (estimatedTargetPosition - transform.position).normalized * projectileSpeed;

            fireCountdown = 1f / turretData.FireRate;
        }

        fireCountdown -= Time.deltaTime;
    }
}

