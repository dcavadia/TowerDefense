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
    private bool isWaveActive = false;

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
        StartNextWave();
    }

    // Update is called once per frame
    void Update()
    {
        if (isWaveActive)
        {
            if (creepsRemainingInWave == 0)
            {
                isWaveActive = false;
                StartNextWave();

            }
        }
    }

    private void SetPlayerData()
    {
        EconomyManager.Instance.SetInitialCoins(PlayerData.startingCoins);
        PlayerManager.Instance.SetInitialHealth(PlayerData.startingHealth);

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
        if (currentWave >= LevelData.waves.Count)
        {
            Debug.Log("All waves complete!");
            if (LastWaveCleared != null)
                LastWaveCleared();


            return;
        }

        WaveData waveData = LevelData.waves[currentWave];

        StartCoroutine(SpawnWave(waveData));

        currentWave++;
    }

    private IEnumerator SpawnWave(WaveData waveData)
    {
        yield return new WaitForSeconds(waveData.waveDelay);

        creepsRemainingInWave = waveData.creepDataArray.Length;

        for (int i = 0; i < waveData.creepDataArray.Length; i++)
        {
            CreepData creepData = waveData.creepDataArray[i].creepData;

            SpawnPointData spawnPoint = SpawnPoints.Find(spawn => spawn.spawnPointId == waveData.creepDataArray[i].spawnPointId);
            Creep creepController = SpawnCreep(creepData, spawnPoint);

            creepsRemainingInWave--;

            yield return new WaitForSeconds(waveData.timeBetweenCreeps);
        }

        isWaveActive = true;
    }

    // Use object pooling to improve performance
    private Creep SpawnCreep(CreepData creepData, SpawnPointData spawnPoint)
    {
        ObjectPool<Creep> pool;
        Component component = creepData.prefab.GetComponent<Creep>();
        Type type = component.GetType();

        if (!creepPools.TryGetValue(type, out pool))
        {
            Debug.LogErrorFormat("No object pool found for type {0}", type);
            return null;
        }

        Creep creepController = pool.GetObjectFromPool();
        if(creepController == null)
        {
            GameObject newCreep = Instantiate(creepData.prefab, spawnPoint.position.transform.position, Quaternion.identity);
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

    // Add creeps to the object pool when they are destroyed
    public void AddCreepToPool(Creep creepController)
    {
        Type type = creepController.GetType();
        ObjectPool<Creep> pool;

        if (!creepPools.TryGetValue(type, out pool))
        {
            Debug.LogErrorFormat("No object pool found for type {0}", type);
            return;
        }

        pool.ReturnObjectToPool(creepController);
    }
}


[Serializable]
public class SpawnPointData
{
    public float spawnPointId;
    public GameObject position;
}


