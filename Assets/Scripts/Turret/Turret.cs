using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
//Abstract Factory, State and Observer patterns
public abstract class Turret : MonoBehaviour, ITurretObserver
{
    protected Creep target;
    protected float range;
    protected TurretState state;
    protected Vector3 currentPosition;
    protected TurretData turretData;

    //Each turret have its own queue of targets
    private Queue<Creep> creepsInRangeQueue = new Queue<Creep>();

    public void Initialize(Vector3 position, Creep target, float range, TurretData turretData)
    {
        this.target = target;
        this.range = range;
        state = new IdleState(this);
        currentPosition = position;
        this.turretData = turretData;
        SetColliderRange(turretData.range);
    }

    public void Update()
    {
        if (state != null)
            state.UpdateState();
    }

    private void SetColliderRange(float range) 
    {
        SphereCollider sphereCollider = GetComponent<SphereCollider>();
        sphereCollider.radius = range;
    }

    public void ChangeState(TurretState state)
    {
        this.state = state;
    }

    public abstract void Shoot();

    public bool TargetInRange()
    {
        if (!target) 
            return false;

        return true;
    }

    public void OnCreepEnteredRange(Creep creep)
    {
        if (state is IdleState && creep != null)
        {
            target = creep;
            Debug.Log("Entered, Attack: ", creep.gameObject);
        }
        else//Is already attacking
        {
            Debug.Log("Entered, Im already attacking, so queue: ", creep.gameObject);
            creepsInRangeQueue.Enqueue(creep);
        }
    }

    public void OnCreepLeftRange(Creep creep)
    {
        if (state is AttackState && creep != null && creep == target)
        {
            if (creepsInRangeQueue.Count > 0)
            {
                Debug.Log("Exit, get next creep in queue: ", creep.gameObject);
                target = creepsInRangeQueue.Dequeue();
            }
            else
            {
                Debug.Log("Exit, no more creep left ", creep.gameObject);
                target = null;
                state = new IdleState(this);
            }
        }
        else if (state is AttackState && creep != null && creep != target)
        {
            Debug.Log("Exit, Im still atacking old target: ", creep.gameObject);
            //Remove creep from queue
            creepsInRangeQueue = new Queue<Creep>(creepsInRangeQueue.Where(x => x != creep));
        }
    }
}

public interface ITurretObserver
{
    void OnCreepEnteredRange(Creep creep);
    void OnCreepLeftRange(Creep creep);
}