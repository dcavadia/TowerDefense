
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(Rigidbody))]
//Abstract Factory, State and Observer patterns
public abstract class Turret : MonoBehaviour, ITurretObserver
{
    protected TurretData turretData;
    protected Creep target;
    private TurretState state;
    private HashSet<Creep> creepsInRange;
    private SphereCollider sphereCollider;

    private void Update()
    {
        if (state != null)
            state.UpdateState();
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

    public void Initialize(TurretData turretData)
    {
        this.turretData = turretData;
        target = null;
        state = new IdleState(this);
        creepsInRange = new HashSet<Creep>();

        SetColliderRange(turretData.Range);
    }

    private void SetColliderRange(float range)
    {
        sphereCollider = GetComponent<SphereCollider>();
        sphereCollider.radius = range;
    }

    public abstract void Shoot();

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

    public void OnCreepEnteredRange(Creep creep)
    {
        if (creep == null || turretData == null || creepsInRange.Contains(creep))
            return;

        creep.CreepKilled += OnCreepKilled;
        creep.CreepReachedBase += OnCreepReachedBase;

        // Start tracking creeps
        creepsInRange.Add(creep);

        try
        {
            if (state is IdleState && target == null)
            {
                target = creep;
            }
            else if (creep != target)
            {//This should be the case when placing a turret with numerous creeps already under its range since unity manage their triggers randomly
                float distanceToCreep = Vector3.Distance(transform.position, creep.transform.position);
                float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);

                if (distanceToCreep < distanceToTarget)
                {
                    // New creep is closer, update the target
                    target = creep;
                }
            }
        }
        catch (NullReferenceException)
        {
            // You can log an error, handle the situation, or take any other appropriate action
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

        target = WaveManager.Instance.SpatialHashGrid.GetNearestCreep(transform.position, turretData.Range);
    }

    protected Projectile SpawnProjectile()
    {
        ObjectPool<Projectile> pool;
        Component component = turretData.Projectile.Prefab.GetComponent<Projectile>();
        Type type = component.GetType();

        if (!ObjectPoolManager.Instance.ProjectilePools.TryGetValue(type, out pool))
        {
            Debug.LogErrorFormat("No object pool found for type {0}", type);
            return null;
        }

        Projectile projectile = pool.GetObjectFromPool();
        if (projectile != null)
        {
            projectile.transform.position = new Vector3(transform.position.x, 1f, transform.position.z);
            projectile.gameObject.SetActive(true);
        }
        else
        {// Pool empty
            GameObject newProjectile = Instantiate(turretData.Projectile.Prefab, new Vector3(transform.position.x, 1f, transform.position.z), Quaternion.identity);
            projectile = newProjectile.GetComponent<Projectile>();
        }

        return projectile;
    }

}

//The Turret script implements the ITurretObserver interface, which defines two methods that get called when a Creep enters or leaves the range of the turret.
//These methods allow the Turret script to react to changes in the Creep's position, which makes it the observer in this case.
public interface ITurretObserver
{
    void OnCreepEnteredRange(Creep creep);
    void OnCreepLeftRange(Creep creep);
}