using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

// Singleton, Publish-Subscribe, Command, Object Pooling Pattern
public class WaveManager : SingletonComponent<WaveManager>
{
    public List<SpawnPointData> SpawnPoints;
    public LevelData LevelData;
    public PlayerData PlayerData;
    public GameObject Base;

    private int currentWave = 0;
    private int creepsRemainingInWave = 0;

    public delegate void LastWaveClearedHandler();
    public event LastWaveClearedHandler LastWaveCleared;

    public SpatialHashGrid SpatialHashGrid { get; private set; }

    // Start match
    void Start()
    {
        // Initialize the shared SpatialHashGrid
        SpatialHashGrid = new SpatialHashGrid(1f);

        SetPlayerData();
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

            // Register the creep in the spatial hash grid
            SpatialHashGrid.AddCreep(creepController);

            yield return new WaitForSeconds(waveData.TimeBetweenCreeps);
        }
    }

    // Use object pooling to improve performance
    private Creep SpawnCreep(CreepData creepData, SpawnPointData spawnPoint)
    {
        ObjectPool<Creep> pool;
        Component component = creepData.Prefab.GetComponent<Creep>();
        Type type = component.GetType();

        if (!ObjectPoolManager.Instance.CreepPools.TryGetValue(type, out pool))
        {
            Debug.LogErrorFormat("No object pool found for type {0}", type);
            return null;
        }

        if(spawnPoint == null)
        {
            Debug.LogErrorFormat("No Spawn Point found, please fix the ID in the Wave Data");
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

        if (!ObjectPoolManager.Instance.CreepPools.TryGetValue(type, out pool))
        {
            Debug.LogErrorFormat("No object pool found for type {0}", type);
            return;
        }

        pool.ReturnObjectToPool(creep);
        CreepDestroyed(creep);
        // Remove the creep from the spatial hash grid
        SpatialHashGrid.RemoveCreep(creep);
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


