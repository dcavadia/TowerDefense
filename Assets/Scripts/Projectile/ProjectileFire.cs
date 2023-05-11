using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileFire : Projectile
{
    // Apply 20% of the original damage
    //public float scorchPercentage = 0.2f;
    // During 2 seconds
    //public float scorchDuration = 2f;

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
        // Apply fire scorch effect to the target 
    }
}
