using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreepBig : Creep
{
    public override void TakeDamage(float damage)
    {
        // Big creeps take half damage
        //base.TakeDamage(damage + 0.5f);
        base.TakeDamage(damage);
    }

}