using System.Collections;
using System.Collections.Generic;
using UnityEditor.MemoryProfiler;
using UnityEngine;

public class Creep : MonoBehaviour
{
    CreepData data;
    float health;
    float speed;
    int coins;

    private Vector3 targetPosition;
    private bool isMoveToWaypointCoroutineRunning = false;

    // Delegate types
    public delegate void CreepKilledHandler(Creep creep);
    public delegate void CreepReachedBaseHandler(Creep creep);

    // Event fields
    public event CreepKilledHandler CreepKilled;
    public event CreepReachedBaseHandler CreepReachedBase;

    public virtual void Init(CreepData creepData, Vector3 basePosition)
    {
        data = creepData;
        health = creepData.baseHealth;
        speed = creepData.baseSpeed;
        targetPosition = basePosition;

        // Apply modifiers
        speed *= creepData.speedModifier;
        health *= creepData.healthModifier;
        coins *= creepData.coinsModifier;
    }

    public virtual void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    protected virtual void ReachedBase()
    {
        if (CreepReachedBase != null)
            CreepReachedBase(this);

        PlayerManager.Instance.ReduceHealth(data.baseDamage);

        ReturnCreepToPool();
    }

    protected virtual void Die()
    {
        if (CreepKilled != null)
            CreepKilled(this);

        EconomyManager.Instance.AddCoin(data.baseCoins);

        ReturnCreepToPool();
    }

    protected virtual void ReturnCreepToPool()
    {
        isMoveToWaypointCoroutineRunning = false;
        WaveManager.Instance.AddCreepToPool(this);
    }

    protected virtual void FixedUpdate()
    {
        if (!isMoveToWaypointCoroutineRunning && gameObject.activeSelf)
        {
            StartCoroutine(MoveToWaypointCoroutine());
            isMoveToWaypointCoroutineRunning = true;
        }
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        Turret turret = other.gameObject.GetComponent<Turret>();
        if (turret != null)
        {
            turret.OnCreepEnteredRange(this);
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        Turret turret = other.gameObject.GetComponent<Turret>();
        if (turret != null)
        {
            turret.OnCreepLeftRange(this);
        }
    }

    //All creeps move in the same way, in the case of each creep moving in a different way, just override this functions in each creep.
    //Coroutine instead of update since that can be expensive and inefficient, especially if you have a lot of creeps in your game.
    protected virtual IEnumerator MoveToWaypointCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f); // wait for 0.1 seconds
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

            if (transform.position == targetPosition)
            {
                // Reached waypoint, get next waypoint
                ReachedBase();
                yield break; // exit coroutine
            }
        }
    }
}
