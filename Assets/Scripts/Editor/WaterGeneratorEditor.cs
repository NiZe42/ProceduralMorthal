using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WaterGenerator))]
public class WaterGeneratorEditor : Editor
{
    private static WaterGenerator currentWaterGenerator;
    private bool placementActive;

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
        placementActive          =  false;
        currentWaterGenerator    =  null;
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        if (!placementActive || !currentWaterGenerator)
        {
            return;
        }

        Event @event = Event.current;

        if (@event.type == EventType.MouseDown && @event.button == 0)
        {
            Ray        ray = HandleUtility.GUIPointToWorldRay(@event.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(
                ray,
                out hit,
                Mathf.Infinity,
                currentWaterGenerator.terrainMask))
            {
                currentWaterGenerator.AddNode(hit.point);
            }

            @event.Use();
        }

        HandleUtility.Repaint();
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var waterGenerator = (WaterGenerator)target;

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Water Body"))
        {
            waterGenerator.AddWaterBody();
        }

        if (GUILayout.Button("Remove Selected"))
        {
            waterGenerator.RemoveWaterBody(waterGenerator.selectedBodyIndex);
        }

        GUILayout.EndHorizontal();

        if (GUILayout.Button("Clear ALL Bodies"))
        {
            waterGenerator.ClearAllWaterBodies();
        }

        if (GUILayout.Button(placementActive ? "Stop Placement" : "Start Node Placement"))
        {
            placementActive = !placementActive;

            if (placementActive)
            {
                currentWaterGenerator    =  waterGenerator;
                Tools.current            =  Tool.None;
                SceneView.duringSceneGui -= OnSceneGUI;
                SceneView.duringSceneGui += OnSceneGUI;
            }
            else
            {
                currentWaterGenerator    =  null;
                SceneView.duringSceneGui -= OnSceneGUI;
            }
        }

        GUILayout.Space(15);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Carve"))
        {
            waterGenerator.Carve();
        }

        if (GUILayout.Button("Reset Terrain"))
        {
            waterGenerator.ResetTerrain();
        }

        GUILayout.EndHorizontal();
    }
}