using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileFreezer : Projectile
{
    // Reduce 20% of speed
    public float freezerPercentage = 0.2f;
    // For 2 seconds
    public float freezerDuration = 2f;

    protected override void ApplyEffect(Creep target)
    {
        // Apply slow of speed
        target.ReduceSpeed(freezerPercentage, freezerDuration);
    }
}
