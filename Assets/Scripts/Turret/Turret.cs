using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Abstract Factory and State patterns
public abstract class Turret : MonoBehaviour
{
    protected Transform target;
    protected float range;
    protected TurretState state;
    protected Vector3 currentPosition;
    protected TurretData turretData;

    public void Initialize(Vector3 position, Transform target, float range, TurretData turretData)
    {
        this.target = target;
        this.range = range;
        state = new IdleState(this);
        currentPosition = position;
        this.turretData = turretData;
    }

    public void Update()
    {
        if (state != null)
            state.UpdateState();
    }

    public void ChangeState(TurretState state)
    {
        this.state = state;
    }

    public abstract void Shoot();

    public bool TargetInRange()
    {
        //float distanceToTarget = Vector3.Distance(currentPosition, target.position);
        //return distanceToTarget <= range;
        return false;
    }

    public float GetRange()
    {
        return range;
    }

    public TurretData GetTurretData()
    {
        return turretData;
    }
}