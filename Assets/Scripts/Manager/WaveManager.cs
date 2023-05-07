using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

//Use a combination of the Observer, Command and Object Pooling patterns
//By using the Observer pattern to notify the turrets of new creeps, and the Command pattern to manage the spawning of creeps,
//the wave manager can be easily modified and extended without affecting other parts of the code.
//With Object pooling, we can avoid the performance overhead of instantiating and destroying objects during gameplay,
//which can lead to significant improvements in the game's framerate and overall performance.
public class WaveManager : SingletonComponent<WaveManager>
{
    public List<SpawnPointData> SpawnPoints;
    public LevelData LevelData;
    public PlayerData PlayerData;
    public GameObject Base;

    // Use the Observer pattern to notify the turrets of new creeps
    //public event Action<CreepController> OnCreepSpawned;

    private int currentWave = 0;
    private int creepsRemainingInWave = 0;

    // List of all the types of Creep
    private List<Type> derivedTypes = new List<Type>();

    // Dictionary to store the object pools as reflections
    private Dictionary<Type, ObjectPool<Creep>> creepPools = new Dictionary<Type, ObjectPool<Creep>>();

    public delegate void LastWaveClearedHandler();
    public event LastWaveClearedHandler LastWaveCleared;

    // Start match
    void Start()
    {
        SetPlayerData();
        CreateObjectPools();
    }

    // Update is called once per frame
    void Update()
    {
        if (creepsRemainingInWave == 0)
        {
            StartNextWave();
        }
    }

    private void SetPlayerData()
    {
        PlayerManager.Instance.SetInitialPlayerData(PlayerData); 
    }
    
    private void CreateObjectPools()
    {
        // Get all the types that derive from Creep
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (Type type in assembly.GetTypes())
            {
                if (type.IsSubclassOf(typeof(Creep)))
                {
                    derivedTypes.Add(type);
                }
            }
        }

        // Create object pools for each type of Creep
        foreach (Type type in derivedTypes)
        {
            ObjectPool<Creep> pool = new ObjectPool<Creep>();
            creepPools[type] = pool;
        }
    }

    private void StartNextWave()
    {
        if (currentWave >= LevelData.Waves.Count)
        {
            //Debug.Log("All waves complete!");
            if (LastWaveCleared != null)
                LastWaveCleared();

            return;
        }

        WaveData waveData = LevelData.Waves[currentWave];
        creepsRemainingInWave = waveData.CreepDataArray.Length;

        StartCoroutine(SpawnWave(waveData));

        currentWave++;
    }

    private IEnumerator SpawnWave(WaveData waveData)
    {
        yield return new WaitForSeconds(waveData.WaveDelay);

        for (int i = 0; i < waveData.CreepDataArray.Length; i++)
        {
            CreepData creepData = waveData.CreepDataArray[i].CreepData;

            SpawnPointData spawnPoint = SpawnPoints.Find(spawn => spawn.spawnPointId == waveData.CreepDataArray[i].SpawnPointId);
            Creep creepController = SpawnCreep(creepData, spawnPoint);

            yield return new WaitForSeconds(waveData.TimeBetweenCreeps);
        }
    }

    // Use object pooling to improve performance
    private Creep SpawnCreep(CreepData creepData, SpawnPointData spawnPoint)
    {
        ObjectPool<Creep> pool;
        Component component = creepData.Prefab.GetComponent<Creep>();
        Type type = component.GetType();

        if (!creepPools.TryGetValue(type, out pool))
        {
            Debug.LogErrorFormat("No object pool found for type {0}", type);
            return null;
        }

        Creep creepController = pool.GetObjectFromPool();
        if(creepController == null)
        {
            GameObject newCreep = Instantiate(creepData.Prefab, spawnPoint.position.transform.position, Quaternion.identity);
            creepController = newCreep.GetComponent<Creep>();
        }
        else
        {
            creepController.transform.position = spawnPoint.position.transform.position;
            creepController.gameObject.SetActive(true);
        }

        // To remove coupling between Creep -> PlayerManager and Creep -> EconomyManager.
        creepController.CreepKilled += EconomyManager.Instance.AddCoin;
        creepController.CreepReachedBase += PlayerManager.Instance.ReduceHealth;
        creepController.CreepReturnToPool += AddCreepToPool;

        creepController.Init(creepData, Base.transform.position);

        return creepController;
    }

    // Add creeps to the object pool when they are destroyed (killed or reached base)
    public void AddCreepToPool(Creep creep)
    {
        Type type = creep.GetType();
        ObjectPool<Creep> pool;

        if (!creepPools.TryGetValue(type, out pool))
        {
            Debug.LogErrorFormat("No object pool found for type {0}", type);
            return;
        }

        pool.ReturnObjectToPool(creep);
        CreepDestroyed(creep);
    }

    private void CreepDestroyed(Creep creep)
    {
        creepsRemainingInWave--;
        creep.CreepKilled -= EconomyManager.Instance.AddCoin;
        creep.CreepReachedBase -= PlayerManager.Instance.ReduceHealth;
        creep.CreepReturnToPool -= AddCreepToPool;
    }
}


[Serializable]
public class SpawnPointData
{
    public float spawnPointId;
    public GameObject position;
}


