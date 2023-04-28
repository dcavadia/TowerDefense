using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creep : MonoBehaviour
{
    public float health;
    public float speed;

    private Vector3 targetPosition;
    //private int targetWaypointIndex;

    // Delegate types
    public delegate void CreepKilledHandler(Creep creep);
    public delegate void CreepReachedBaseHandler(Creep creep);

    // Event fields
    public event CreepKilledHandler CreepKilled;
    public event CreepReachedBaseHandler CreepReachedBase;

    public void TakeDamage(float damage)
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

    protected virtual void Update()
    {
        MoveToWaypoint();
    }

    private void MoveToWaypoint()
    {
        /*if (targetWaypointIndex >= Waypoints.Instance.waypoints.Length)
        {
            // Reached end of path
            return;
        }*/

        //targetPosition = Waypoints.Instance.waypoints[targetWaypointIndex].position;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        /*if (transform.position == targetPosition)
        {
            targetWaypointIndex++;
        }*/
    }
}
