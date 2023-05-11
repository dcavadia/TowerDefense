using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EconomyManager : SingletonComponent<EconomyManager>
{
    private float coins = 0f;

    public void SetInitialCoins(int amount)
    {
        coins = amount;
    }

    public bool CanAffordTurret(float cost)
    {
        return coins >= cost;
    }

    public void PurchaseTurret(float cost)
    {
        coins -= cost;
    }

    public void AddCoin(Creep creep)
    {
        coins += creep.GetData().CoinsReward;
        creep.CreepKilled -= AddCoin;
    }

    public float GetPlayerCoins()
    {
        return coins;
    }
}
