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
    public CreepBattle[] creepDataArray; // An array of CreepData to spawn in this wave
    public float timeBetweenCreeps; // The time to wait between spawning each creep
    public float waveDelay; // The time to wait before starting this wave
}

[Serializable]
public class CreepBattle
{
    public CreepData creepData;
    public float spawnPointId;
}