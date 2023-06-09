using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Abstract and Publish-Subscribe pattern
public abstract class Creep : MonoBehaviour
{
    // Encapsulation promotes data integrity :)
    private CreepData data;
    private float health;
    private float speed;

    private Vector3 targetPosition;
    private bool isMoveToWaypointCoroutineRunning = false;

    // Delegate types
    public delegate void CreepKilledHandler(Creep creep);
    public delegate void CreepReachedBaseHandler(Creep creep);
    public delegate void CreepReturnToPoolHandler(Creep creep);

    // Event fields
    public event CreepKilledHandler CreepKilled;
    public event CreepReachedBaseHandler CreepReachedBase;
    public event CreepReturnToPoolHandler CreepReturnToPool;

    [SerializeField] private Image healthBarImage;

    private void FixedUpdate()
    {
        if (!isMoveToWaypointCoroutineRunning && gameObject.activeSelf)
        {
            StartCoroutine(MoveToWaypointCoroutine());
            isMoveToWaypointCoroutineRunning = true;
        }
    }

    public void Initialize(CreepData creepData, Vector3 basePosition)
    {
        data = creepData;
        health = creepData.Health;
        speed = creepData.Speed;
        targetPosition = basePosition;

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

    public void ReduceSpeed(float percentage, float duration)
    {
        if (gameObject.activeSelf)
            StartCoroutine(ReduceSpeedCoroutine(percentage, duration));
    }

    protected virtual IEnumerator MoveToWaypointCoroutine()
    {
        float distanceThreshold = 0.01f;
        WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        Vector3 moveDirection = Vector3.zero;
        Quaternion targetRotation = Quaternion.identity;

        while (true)
        {
            float step = speed * Time.deltaTime;

            WaveManager.Instance.SpatialHashGrid.RemoveCreep(this);
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);
            WaveManager.Instance.SpatialHashGrid.AddCreep(this);

            moveDirection = targetPosition - transform.position;
            if (moveDirection != Vector3.zero)
            {
                targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = targetRotation;
            }

            if (Vector3.Distance(transform.position, targetPosition) <= distanceThreshold)
            {
                ReachedBase();
                yield break;
            }

            // Instead of yield return null, this uses a pre-allocated YieldInstruction object reducing GC alloc.
            yield return waitForFixedUpdate;
        }
    }

    private IEnumerator ReduceSpeedCoroutine(float percentage, float duration)
    {
        float amountReduced = data.Speed * percentage;

        if (speed - amountReduced <= 0)
        {
            // Already frozen
            yield break;
        }

        speed -= amountReduced;

        yield return new WaitForSeconds(duration);

        speed += amountReduced;
    }


    private void UpdateHealthBar()
    {
        if (healthBarImage != null)
        {
            float fillAmount = health / data.Health;
            healthBarImage.fillAmount = fillAmount;
        }
    }

    private void ReachedBase()
    {
        CreepReachedBase?.Invoke(this);
        ReturnCreepToPool();
    }

    private void Die()
    {
        CreepKilled?.Invoke(this);
        ReturnCreepToPool();
    }

    private void ReturnCreepToPool()
    {
        CreepReturnToPool?.Invoke(this);
    }

    public CreepData GetData()
    {
        return data;
    }
}