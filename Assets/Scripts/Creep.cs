using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creep : MonoBehaviour
{
    public CreepData data;

    public float health;
    public float speed;

    private Vector3 targetPosition;
    private bool coroutineStarted = false;

    // Delegate types
    public delegate void CreepKilledHandler(Creep creep);
    public delegate void CreepReachedBaseHandler(Creep creep);

    // Event fields
    public event CreepKilledHandler CreepKilled;
    public event CreepReachedBaseHandler CreepReachedBase;

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
        Destroy(gameObject);
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
