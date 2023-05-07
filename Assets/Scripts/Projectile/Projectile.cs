using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Separate the turret logic from the projectile logic, allowing for easy extension and customization.
public class Projectile : MonoBehaviour
{
    private float damage;
    private bool hasHitTarget = false;

    public void SetDamage(float damage)
    {
        this.damage = damage;
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
            Destroy(gameObject);
        }
    }

}
