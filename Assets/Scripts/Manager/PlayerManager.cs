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
        health -= creep.data.Damage;
        if (health <= 0)
        {
            GameOver();
        }
        //creep.CreepReachedBase -= ReduceHealth;
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
