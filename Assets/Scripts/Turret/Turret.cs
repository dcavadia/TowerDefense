using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TreeEditor;
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
    protected List<Creep> creepsInRange;

    public void Initialize(Vector3 position, Creep target, float range, TurretData turretData)
    {
        this.target = target;
        this.range = range;
        state = new IdleState(this);
        currentPosition = position;
        this.turretData = turretData;
        creepsInRange = new List<Creep>();

        SetColliderRange(turretData.Range);
    }

    private void SetColliderRange(float range) 
    {
        SphereCollider sphereCollider = GetComponent<SphereCollider>();
        sphereCollider.radius = range;
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        Creep creep = other.gameObject.GetComponent<Creep>();
        if (creep != null && turretData != null)
        {
            OnCreepEnteredRange(creep);
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        Creep creep = other.gameObject.GetComponent<Creep>();
        if (creep != null && turretData != null)
        {
            OnCreepLeftRange(creep);
        }
    }

    public abstract void Shoot();

    public void Update()
    {
        if (state != null)
            state.UpdateState();
    }

    public void ChangeState(TurretState state)
    {
        this.state = state;
    }

    public bool TargetInRange()
    {
        if (!target) 
            return false;

        return true;
    }

    //TODO: Maybe move to a separate class responsible for managing the queue, so that the Turret class does not need to handle queue management?. 
    public void OnCreepEnteredRange(Creep creep)
    {
        if (creep == null || turretData == null || creepsInRange.Contains(creep))
            return;

        creep.CreepKilled += OnCreepKilled;
        creep.CreepReachedBase += OnCreepReachedBase;

        // Add the creep to the spatial hash grid
        creepsInRange.Add(creep);

        try
        {
            if (state is IdleState && target == null)
            {
                target = creep;
            }
            else if (creep != target)
            {//This should be the case when placing a turret with numerous creeps already under its range so unity manage their triggers randomly
                float distanceToCreep = Vector3.Distance(transform.position, creep.transform.position);
                float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);

                if (distanceToCreep < distanceToTarget)
                {
                    // New creep is closer, update the target
                    target = creep;
                }
            }
        }
        catch (NullReferenceException e)
        {
            // Handle the null reference exception here
            // You can log an error, handle the situation, or take any other appropriate action
            Debug.LogError("Null reference exception: " + e.Message);
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
        if (creep == null || !creepsInRange.Contains(creep))
            return;

        creep.CreepKilled -= OnCreepKilled;
        creep.CreepReachedBase -= OnCreepReachedBase;

        creepsInRange.Remove(creep);
        if (creepsInRange.Count <= 0)
            target = null;
    }

    protected void CheckNearestTarget()
    {
        if (creepsInRange.Count <= 0)
            return;

        target = WaveManager.Instance.SpatialHashGrid.GetNearestCreep(transform.position, range);
    }

    protected Projectile SpawnProjectile()
    {
        ObjectPool<Projectile> pool;
        Component component = turretData.Projectile.GetComponent<Projectile>();
        Type type = component.GetType();

        if (!ObjectPoolManager.Instance.ProjectilePools.TryGetValue(type, out pool))
        {
            Debug.LogErrorFormat("No object pool found for type {0}", type);
            return null;
        }

        Projectile projectile = pool.GetObjectFromPool();
        if (projectile == null)
        {
            GameObject newProjectile = Instantiate(turretData.Projectile, new Vector3(transform.position.x, 1f, transform.position.z), Quaternion.identity);
            projectile = newProjectile.GetComponent<Projectile>();
        }
        else
        {
            projectile.transform.position = new Vector3(transform.position.x, 1f, transform.position.z);
            projectile.gameObject.SetActive(true);
        }

        return projectile;
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