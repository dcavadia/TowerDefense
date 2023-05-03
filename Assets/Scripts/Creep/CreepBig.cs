using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreepBig : Creep
{
    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
    }

    protected override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);
    }

    public override void TakeDamage(float damage)
    {
        // Big creeps take half damage
        base.TakeDamage(damage * 0.5f);
    }
}
