using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Creep", menuName = "Tower Defense/Create New Creep")]
public class CreepData : ScriptableObject
{
    [Header("Base Stats")]
    [SerializeField] private float health;
    [SerializeField] private float speed;
    [SerializeField] private int coinsReward;
    [SerializeField] private int damage;

    [SerializeField] private GameObject prefab;

    //Public getters for each variable
    public float Health { get { return health; } }
    public float Speed { get { return speed; } }
    public int CoinsReward { get { return coinsReward; } }
    public int Damage { get { return damage; } }
    public GameObject Prefab { get { return prefab; } }
}
