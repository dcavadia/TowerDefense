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

    //Each turret have its own queue of targets.
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

    //TODO: Move to a separate class responsible for managing the queue, so that the Turret class does not need to handle queue management. 
    public void OnCreepEnteredRange(Creep creep)
    {
        if (creepsInRangeQueue.Contains(creep))
            return;

        creep.CreepKilled += OnCreepKilled;
        creep.CreepReachedBase += OnCreepReachedBase;

        if (state is IdleState && creep != null)
        {
            target = creep;
        }
        else if(creep != target)//Is already attacking
        {
            creepsInRangeQueue.Enqueue(creep);
        }
        
    }

    public void OnCreepLeftRange(Creep creep)
    {
        RemoveTrackOfCreep(creep);
    }

    private void OnCreepKilled(Creep creep)
    {
        RemoveTrackOfCreep(creep);
    }

    private void OnCreepReachedBase(Creep creep)
    {
        RemoveTrackOfCreep(creep);
    }

    private void RemoveTrackOfCreep(Creep creep)
    {
        if (state is AttackState && creep != null && creep == target)
        {
            if (creepsInRangeQueue.Count > 0)
            {
                target = creepsInRangeQueue.Dequeue();

                //Remove creep from queue
                if (creepsInRangeQueue.Contains(creep))
                    creepsInRangeQueue = new Queue<Creep>(creepsInRangeQueue.Where(x => x != creep));
                //TODO: Consider HashSet instead of queue?
            }
            else
            {
                target = null;
                state = new IdleState(this);
            }
        }
        else if (state is AttackState && creep != null && creep != target)
        {
            //Remove creep from queue
            if (creepsInRangeQueue.Contains(creep))
                creepsInRangeQueue = new Queue<Creep>(creepsInRangeQueue.Where(x => x != creep));
        }
        creep.CreepKilled -= OnCreepKilled;
        creep.CreepReachedBase -= OnCreepReachedBase;
    }
}

//The Turret script is using the Observer pattern, where it observes the Creep script's position in the game world. The Creep script is raising events when it enters or leaves the range of a turret.
//The Turret script implements the ITurretObserver interface, which defines two methods that get called when a Creep enters or leaves the range of the turret.
//These methods allow the Turret script to react to changes in the Creep's position, which makes it the observer in this case.
public interface ITurretObserver
{
    void OnCreepEnteredRange(Creep creep);
    void OnCreepLeftRange(Creep creep);
}