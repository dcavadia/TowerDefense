using System.Collections;
using System.Collections.Generic;
using UnityEditor.MemoryProfiler;
using UnityEngine;
using UnityEngine.UI;

public class Creep : MonoBehaviour
{
    public CreepData data { get; private set; }
    float health;
    float speed;

    [SerializeField] private Image healthBarImage;

    private Vector3 targetPosition;
    private bool isMoveToWaypointCoroutineRunning = false;

    // Publish-Subscribe pattern
    // Delegate types
    public delegate void CreepKilledHandler(Creep creep);
    public delegate void CreepReachedBaseHandler(Creep creep);
    public delegate void CreepReturnToPoolHandler(Creep creep);

    // Event fields
    public event CreepKilledHandler CreepKilled;
    public event CreepReachedBaseHandler CreepReachedBase;
    public event CreepReturnToPoolHandler CreepReturnToPool;

    public virtual void Init(CreepData creepData, Vector3 basePosition)
    {
        data = creepData;
        health = creepData.Health;
        speed = creepData.Speed;
        targetPosition = basePosition;

        // Apply modifiers
        speed *= creepData.SpeedModifier;
        health *= creepData.HealthModifier;
        isMoveToWaypointCoroutineRunning = false;
        UpdateHealthBar();
    }

    public virtual void TakeDamage(float damage)
    {
        health -= damage;
        UpdateHealthBar();
        if (health <= 0)
        {
            Die();
        }
    }

    public virtual void ReduceSpeed(float percentage, float duration)
    {
        if(gameObject.activeSelf)
            StartCoroutine(ReduceSpeedCoroutine(percentage, duration));
    }

    protected virtual void ReachedBase()
    {
        if (CreepReachedBase != null)
            CreepReachedBase(this);

        ReturnCreepToPool();
    }

    protected virtual void Die()
    {
        if (CreepKilled != null)
            CreepKilled(this);

        ReturnCreepToPool();
    }

    protected virtual void ReturnCreepToPool()
    {
        if (CreepReturnToPool != null)
            CreepReturnToPool(this);
    }

    protected virtual void FixedUpdate()
    {
        if (!isMoveToWaypointCoroutineRunning && gameObject.activeSelf)
        {
            StartCoroutine(MoveToWaypointCoroutine());
            isMoveToWaypointCoroutineRunning = true;
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

    protected virtual IEnumerator ReduceSpeedCoroutine(float percentage, float duration)
    {
        float amountReduced = speed * percentage;

        speed -= amountReduced;

        yield return new WaitForSeconds(duration);

        speed += amountReduced;
    }

    private void UpdateHealthBar()
    {
        if (healthBarImage != null)
        {
            // Calculate the fill amount based on current health
            float fillAmount = health / data.Health;

            // Set the fill amount of the health bar image
            healthBarImage.fillAmount = fillAmount;
        }
    }

}
