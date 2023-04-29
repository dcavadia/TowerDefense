using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Scriptable object that holds the base stats for each creep, as well as any modifiers that may be applied to those stats.
//These modifiers can be used to adjust the stats of individual creeps, or to apply buffs or debuffs to groups of creeps.
[CreateAssetMenu(fileName = "New Creep", menuName = "Tower Defense/Create New Creep")]
public class CreepData : ScriptableObject
{
    public GameObject prefab;

    [Header("Base Stats")]
    public float baseSpeed;
    public float baseHealth;
    public int baseCoins;

    [Header("Modifiers")]
    public float speedModifier = 1f;
    public float healthModifier = 1f;
    public int coinsModifier = 1;
}
