using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretFreezer : Turret 
{ 
    private float fireRate = 1f;
    private float fireCountdown = 0f;
    private Transform partToRotate;

    public override void Shoot()
    {
        // implementation of shooting logic for this turret type
    }
}

