using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WorldGenerator))]
public class WorldGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var worldGenerator = target as WorldGenerator;
        DrawDefaultInspector();

        if (GUILayout.Button("GenerateTerrain"))
        {
            worldGenerator.InitializeAndGenerateTerrain();
        }

        if (GUILayout.Button("GenerateAll"))
        {
            worldGenerator.InitializeAndGenerateAll();
        }
    }
}