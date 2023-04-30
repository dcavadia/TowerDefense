using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Level", menuName = "Tower Defense/Create New Level")]
public class LevelData : ScriptableObject
{
    [Tooltip("First wave to start is Element 0")]
    public List<WaveData> waves;
}
