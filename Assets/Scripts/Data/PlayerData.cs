using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Player", menuName = "Tower Defense/Create New Player")]
public class PlayerData : ScriptableObject
{
    public int startingHealth;
    public int startingCoins;
}
