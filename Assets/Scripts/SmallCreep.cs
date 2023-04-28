using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallCreep : Creep
{
    public override void TakeDamage(float damage)
    {
        // Small creeps take double damage
        base.TakeDamage(damage * 2);
    }
}
