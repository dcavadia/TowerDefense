using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Abstract and Publish-Subscribe pattern
public abstract class Creep : MonoBehaviour
{
    // Encapsulation promotes data integrity :)
    protected CreepData data { get; private set; }
    protected float health;
    protected float speed;

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

    [SerializeField] protected Image healthBarImage;

    public virtual void Init(CreepData creepData, Vector3 basePosition)
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

    public virtual void ReduceSpeed(float percentage, float duration)
    {
        if (gameObject.activeSelf)
            StartCoroutine(ReduceSpeedCoroutine(percentage, duration));
    }

    protected virtual void ReachedBase()
    {
        CreepReachedBase?.Invoke(this);

        ReturnCreepToPool();
    }

    protected virtual void Die()
    {
        CreepKilled?.Invoke(this);

        ReturnCreepToPool();
    }

    protected virtual void ReturnCreepToPool()
    {
        CreepReturnToPool?.Invoke(this);
    }

    protected virtual void FixedUpdate()
    {
        if (!isMoveToWaypointCoroutineRunning && gameObject.activeSelf)
        {
            StartCoroutine(MoveToWaypointCoroutine());
            isMoveToWaypointCoroutineRunning = true;
        }
    }

    protected virtual IEnumerator MoveToWaypointCoroutine()
    {
        float distanceThreshold = 0.01f;

        while (true)
        {
            float step = speed * Time.deltaTime;

            WaveManager.Instance.SpatialHashGrid.RemoveCreep(this);
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);
            WaveManager.Instance.SpatialHashGrid.AddCreep(this);

            Vector3 direction = targetPosition - transform.position;

            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }

            if (Vector3.Distance(transform.position, targetPosition) <= distanceThreshold)
            {
                ReachedBase();
                yield break;
            }

            yield return null;
        }
    }

    protected virtual IEnumerator ReduceSpeedCoroutine(float percentage, float duration)
    {
        float amountReduced = data.Speed * percentage;
        speed -= amountReduced;

        yield return new WaitForSeconds(duration);

        speed += amountReduced;
    }

    protected void UpdateHealthBar()
    {
        if (healthBarImage != null)
        {
            float fillAmount = health / data.Health;
            healthBarImage.fillAmount = fillAmount;
        }
    }

    public CreepData GetData()
    {
        return data;
    }
}