using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : SingletonComponent<PlayerManager>
{
    private float health = 0f;

    private void Start()
    {
        WaveManager.Instance.LastWaveCleared += GameWin;
    }

    public void SetInitialPlayerData(PlayerData player)
    {
        health = player.StartingHealth;
        EconomyManager.Instance.SetInitialCoins(player.StartingCoins);
    }

    public void ReduceHealth(Creep creep)
    {
        health -= creep.GetData().Damage;
        if (health <= 0)
        {
            GameOver();
        }
    }

    public float GetPlayerHealth()
    {
        return health;
    }

    // Consider adding events for these
    private void GameOver()
    {
        UIManager.Instance.LosePopUp();
        PauseGame();
    }

    private void GameWin()
    {
        UIManager.Instance.WinPopUp();
        PauseGame();
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
    }

}
