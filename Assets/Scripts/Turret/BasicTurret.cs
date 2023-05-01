using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicTurret : Turret
{
    private float fireRate = 1f;
    private float fireCountdown = 0f;
    private Transform partToRotate;

    public BasicTurret(Vector3 position, Transform target, float range) : base(position, target, range)
    {
        // initialize partToRotate and other variables specific to this turret type
    }

    public override void Shoot()
    {
        // implementation of shooting logic for this turret type
    }
}

