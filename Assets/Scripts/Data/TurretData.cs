using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Turret", menuName = "Tower Defense/Create New Turret")]
public class TurretData : ScriptableObject
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private GameObject projectile;
    [SerializeField] private float cost;
    [SerializeField] private float range;
    [SerializeField] private float fireRate;
    [SerializeField] private float damage;

    //Public getters
    public GameObject Prefab { get { return prefab; } }
    public GameObject Projectile { get { return projectile; } }
    public float Cost { get { return cost; } }
    public float Range { get { return range; } }
    public float FireRate { get { return fireRate; } }
    public float Damage { get { return damage; } }
}
