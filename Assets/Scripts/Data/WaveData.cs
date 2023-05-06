using System;
using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;


//Scriptable object for each wave, containing the information about the creeps that spawn in that wave, the number of creeps, and any other relevant data
//Can be easily edited and tweaked by designers and level designers.
[CreateAssetMenu(fileName = "New Wave", menuName = "Tower Defense/Create New Wave", order = 1)]
public class WaveData : ScriptableObject
{
    [Tooltip("First creep to go out is Element 0")]
    [SerializeField] private CreepBattle[] creepDataArray; // An array of CreepData to spawn in this wave
    [SerializeField] private float timeBetweenCreeps; // The time to wait between spawning each creep
    [SerializeField] private float waveDelay; // The time to wait before starting this wave

    //Public getters
    public CreepBattle[] CreepDataArray { get { return creepDataArray; } }
    public float TimeBetweenCreeps { get { return timeBetweenCreeps; } }
    public float WaveDelay { get { return waveDelay; } }
}

[Serializable]
public class CreepBattle
{
    [SerializeField] private CreepData creepData;
    [SerializeField] private float spawnPointId;

    public CreepData CreepData { get { return creepData; } }
    public float SpawnPointId { get { return spawnPointId; } }
}