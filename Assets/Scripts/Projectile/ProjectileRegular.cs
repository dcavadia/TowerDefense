using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileRegular : Projectile
{
    protected override void OnTriggerEnter(Collider other)
    {
        Creep creep = other.gameObject.GetComponent<Creep>();
        if (creep != null)
        {
            base.OnTriggerEnter(other);
            ApplyEffect(creep);
        }
    }

    protected override void ApplyEffect(Creep target)
    {
        // No effects
    }
}
