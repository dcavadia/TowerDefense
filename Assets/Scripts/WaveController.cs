using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Use a combination of the Observer, Command and Object Pooling patterns
//By using the Observer pattern to notify the turrets of new creeps, and the Command pattern to manage the spawning of creeps,
//the wave controller can be easily modified and extended without affecting other parts of the code.
//With Object pooling, we can avoid the performance overhead of instantiating and destroying objects during gameplay,
//which can lead to significant improvements in the game's framerate and overall performance.
public class WaveController : SingletonComponent<WaveController>
{
    public List<SpawnPointData> SpawnPoints;
    public LevelData LevelData;

    // Use the Observer pattern to notify the turrets of new creeps
    //public event Action<CreepController> OnCreepSpawned;

    private int currentWave = 0;
    private int creepsRemainingInWave = 0;
    private bool isWaveActive = false;

    private Queue<Creep> creepPool = new Queue<Creep>();


    // Start is called before the first frame update
    void Start()
    {
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

    private void StartNextWave()
    {
        if (currentWave >= LevelData.waves.Count)
        {
            Debug.Log("All waves complete!");
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
        Creep creepController;

        if (creepPool.Count > 0)
        {
            // Use an existing creep from the pool
            creepController = creepPool.Dequeue();
            creepController.gameObject.SetActive(true);
            creepController.transform.position = spawnPoint.position.transform.position;
        }
        else
        {
            // Create a new creep
            GameObject newCreep = Instantiate(creepData.prefab, spawnPoint.position.transform.position, Quaternion.identity);
            creepController = newCreep.GetComponent<Creep>();
            //creepController.Init(creepData);
        }

        return creepController;
    }

    // Add creeps to the object pool when they are destroyed
    public void AddCreepToPool(Creep creepController)
    {
        creepController.gameObject.SetActive(false);
        creepPool.Enqueue(creepController);
    }


}


[Serializable]
public class SpawnPointData
{
    public float spawnPointId;
    public GameObject position;
}


