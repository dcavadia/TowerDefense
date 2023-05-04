using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Turret", menuName = "Tower Defense/Create New Turret")]
public class TurretData : ScriptableObject
{
    public GameObject prefab;
    public GameObject projectile;
    public float range;
    public float fireRate;
    public float damage;
}
