using System;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "New Wave", menuName = "Tower Defense/Create New Wave", order = 1)]
public class WaveData : ScriptableObject
{
    [Header("Creeps start from top to bottom\n")]
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




#if UNITY_EDITOR
// Custom property drawer to improve the display of CreepBattle in the inspector
[CustomPropertyDrawer(typeof(CreepBattle))]
public class CreepBattleDrawer : PropertyDrawer
{
    private const float DescriptorWidth = 100f;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Divide the available width
        float spawnPointWidth = position.width * 0.25f;

        // Calculate the position of the SpawnPointId field
        Rect spawnPointIdRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing, spawnPointWidth - EditorGUIUtility.standardVerticalSpacing, EditorGUIUtility.singleLineHeight);

        // Calculate the position of the CreepData field
        Rect creepDataRect = new Rect(position.x + spawnPointWidth, position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing, position.width - spawnPointWidth - EditorGUIUtility.standardVerticalSpacing, EditorGUIUtility.singleLineHeight);

        // Calculate the position of the SpawnPointId descriptor
        Rect spawnPointIdDescriptorRect = new Rect(position.x, position.y, DescriptorWidth, EditorGUIUtility.singleLineHeight);

        // Calculate the position of the CreepData descriptor
        Rect creepDataDescriptorRect = new Rect(position.x + spawnPointWidth, position.y, DescriptorWidth * 3, EditorGUIUtility.singleLineHeight);

        // Get the serialized properties of the fields
        SerializedProperty creepDataProperty = property.FindPropertyRelative("creepData");
        SerializedProperty spawnPointIdProperty = property.FindPropertyRelative("spawnPointId");

        // Display the SpawnPointId descriptor
        EditorGUI.LabelField(spawnPointIdDescriptorRect, "Spawn Point Id");

        // Display the SpawnPointId field
        EditorGUI.PropertyField(spawnPointIdRect, spawnPointIdProperty, GUIContent.none);

        // Display the CreepData descriptor
        EditorGUI.LabelField(creepDataDescriptorRect, "Creep Data");

        // Display the CreepData field
        EditorGUI.PropertyField(creepDataRect, creepDataProperty, GUIContent.none);

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing;
    }
}
#endif