using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : SingletonComponent<PlayerManager>
{
    private float health = 0f;

    public void SetInitialHealth(int amount)
    {
        health = amount;
    }

    public void ReduceHealth(float amount)
    {
        health -= amount;
    }

    public float GetPlayerHealth()
    {
        return health;
    }

}
