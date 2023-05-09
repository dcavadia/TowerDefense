using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class ObjectPoolManager : SingletonComponent<ObjectPoolManager>
{
    // Dictionary to store the object pools as reflections
    public Dictionary<Type, ObjectPool<Creep>> CreepPools { get; private set; } = new Dictionary<Type, ObjectPool<Creep>>();
    public Dictionary<Type, ObjectPool<Projectile>> ProjectilePools { get; private set; } = new Dictionary<Type, ObjectPool<Projectile>>();

    // List of all the types of Creep and Projectile
    private List<Type> derivedTypes  = new List<Type>();

    // Start is called before the first frame update
    void Start()
    {
        CreateObjectPools();
    }

    // If there are a large number of assemblies, it can have a noticeable impact on performance. Current assemblies: 156
    // It occurs only once during initialization, so it is unlikely to significantly affect the overall performance of the game.
    private void CreateObjectPools()
    {
        // Get all the types that derive from Creep and Projectiles
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (Type type in assembly.GetTypes())
            {
                if (type.IsSubclassOf(typeof(Creep)) || type.IsSubclassOf(typeof(Projectile)))
                {
                    derivedTypes.Add(type);
                }
            }
        }

        // Create object pools for each type of Creep
        foreach (Type type in derivedTypes)
        {
            if (type.IsSubclassOf(typeof(Creep)))
            {
                ObjectPool<Creep> pool = new ObjectPool<Creep>();
                CreepPools[type] = pool;
            }
            else if (type.IsSubclassOf(typeof(Projectile)))
            {
                ObjectPool<Projectile> pool = new ObjectPool<Projectile>();
                ProjectilePools[type] = pool;
            } 
        }
    }
}
