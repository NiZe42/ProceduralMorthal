using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RoadGenerator))]
public class RoadGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var roadGenerator = (RoadGenerator)target;

        GUILayout.Space(10);
        if (GUILayout.Button("Clear all roads"))
        {
            roadGenerator.ClearAllRoads();
        }
    }
}