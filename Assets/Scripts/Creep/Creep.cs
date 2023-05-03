using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creep : MonoBehaviour
{
    CreepData data;
    float health;
    float speed;
    int coins;

    private Vector3 targetPosition;
    private bool coroutineStarted = false;

    // Delegate types
    public delegate void CreepKilledHandler(Creep creep);
    public delegate void CreepReachedBaseHandler(Creep creep);

    // Event fields
    public event CreepKilledHandler CreepKilled;
    public event CreepReachedBaseHandler CreepReachedBase;
    

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
            // Invoke CreepKilled event
            if (CreepKilled != null)
            {
                CreepKilled(this);
            }
            Die();
        }
    }

    public void ReachedBase()
    {
        // Invoke CreepReachedBase event
        if (CreepReachedBase != null)
        {
            CreepReachedBase(this);
        }
        Die();
    }

    protected virtual void Die()
    {
        // Play death animation, remove from scene, etc.
        WaveController.Instance.AddCreepToPool(this);
        //Destroy(gameObject);
    }

    protected virtual void FixedUpdate()
    {
        if (!coroutineStarted)
        {
            StartCoroutine(MoveToWaypointCoroutine());
            coroutineStarted = true;
        }
    }

    //Set it as a coroutine instead of update since this can be expensive and inefficient, especially if you have a lot of creeps in your game. 
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
                coroutineStarted = false;
                yield break; // exit coroutine
            }
        }
    }
}
