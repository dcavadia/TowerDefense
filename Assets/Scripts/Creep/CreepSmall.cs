using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreepSmall : Creep
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
        // Small creeps take double damage
        base.TakeDamage(damage * 2);
    }
}
