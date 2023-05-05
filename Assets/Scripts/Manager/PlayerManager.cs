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
            GameOver();
        }
    }

    public float GetPlayerHealth()
    {
        return health;
    }

    private void GameOver()
    {
        UIManager.Instance.LosePopUp();
        Time.timeScale = 0f;
    }

    private void GameWin()
    {
        UIManager.Instance.WinPopUp();
        Time.timeScale = 0f;
    }

}
