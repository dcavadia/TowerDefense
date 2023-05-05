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
        if (health <= 0)
        {
            // Player has lost the game
            //GameManager.Instance.GameOver();
            Debug.Log("GameOver");
        }
    }

    public float GetPlayerHealth()
    {
        return health;
    }

}
