using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Player", menuName = "Tower Defense/Create New Player")]
public class PlayerData : ScriptableObject
{
    [SerializeField] private int startingHealth;
    [SerializeField] private int startingCoins;

    //Public getters
    public int StartingHealth { get { return startingHealth; } }
    public int StartingCoins { get { return startingCoins; } }
}
