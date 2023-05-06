using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Scriptable object that holds the base stats for each creep, as well as any modifiers that may be applied to those stats.
//These modifiers can be used to adjust the stats of individual creeps, or to apply buffs or debuffs to groups of creeps.
[CreateAssetMenu(fileName = "New Creep", menuName = "Tower Defense/Create New Creep")]
public class CreepData : ScriptableObject
{
    [SerializeField] private GameObject prefab;

    [Header("Base Stats")]
    [SerializeField] private float speed;
    [SerializeField] private float health;
    [SerializeField] private int coinsReward;
    [SerializeField] private int damage;

    [Header("Modifiers")]
    [SerializeField] private float speedModifier = 1f;
    [SerializeField] private float healthModifier = 1f;
    [SerializeField] private int coinsModifier = 1;

    //Public getters for each variable
    public GameObject Prefab { get { return prefab; } }
    public float Speed { get { return speed; } }
    public float Health { get { return health; } }
    public int CoinsReward { get { return coinsReward; } }
    public int Damage { get { return damage; } }

    public float SpeedModifier { get { return speedModifier; } }
    public float HealthModifier { get { return healthModifier; } }
    public int CoinsModifier { get { return coinsModifier; } }
}
