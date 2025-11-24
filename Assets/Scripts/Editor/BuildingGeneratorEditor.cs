using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BuildingGenerator))]
public class BuildingGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var buildingGenerator = (BuildingGenerator)target;

        GUILayout.Space(10);
        if (GUILayout.Button("Clear all buildings"))
        {
            buildingGenerator.ClearBuildings();
        }
    }
}