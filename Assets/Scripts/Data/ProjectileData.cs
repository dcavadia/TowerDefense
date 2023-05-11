using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Projectile", menuName = "Tower Defense/Create New Projectile")]
public class ProjectileData : ScriptableObject
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private float damage;

    //Public getters
    public GameObject Prefab { get { return prefab; } }
    public float Damage { get { return damage; } }
}
