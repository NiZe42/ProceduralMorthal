using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

[CustomEditor(typeof(WorldGenerator))]
public class WorldGeneratorEditor : Editor
{
    private const string TAB_KEY = "WorldGeneratorEditor_LastTab";

    private readonly string[] tabs = { "Terrain", "Water", "Roads", "Buildings", "All" };

    private Editor buildingEditor;
    private Editor roadEditor;

    private bool showBaseSettings;
    private bool showReferences;
    private int tabIndex;

    private Editor terrainEditor;
    private Editor waterEditor;

    private void OnEnable()
    {
        var manager = target as WorldGenerator;
        tabIndex = EditorPrefs.GetInt(TAB_KEY, 0);
        if (!WorldGenerator.Instance)
        {
            WorldGenerator.Instance = target as WorldGenerator;
        }
        else if (WorldGenerator.Instance != target as WorldGenerator)
        {
            Debug.LogWarning("Multiple WorldGeneratorEditor instances detected.");
        }
    }

    private void OnDisable()
    {
        DestroyEditor(ref terrainEditor);
        DestroyEditor(ref waterEditor);
        DestroyEditor(ref roadEditor);
        DestroyEditor(ref buildingEditor);
        if (WorldGenerator.Instance == target as WorldGenerator)
        {
            WorldGenerator.Instance = null;
        }
    }

    public override void OnInspectorGUI()
    {
        var worldGenerator = target as WorldGenerator;

        showReferences = EditorGUILayout.Foldout(showReferences, "References");
        if (showReferences)
        {
            worldGenerator.terrainGenerator = (TerrainGenerator)EditorGUILayout.ObjectField(
                "Terrain Generator",
                worldGenerator.terrainGenerator,
                typeof(TerrainGenerator),
                true);

            worldGenerator.waterGenerator = (WaterGenerator)EditorGUILayout.ObjectField(
                "Water Generator",
                worldGenerator.waterGenerator,
                typeof(WaterGenerator),
                true);

            worldGenerator.roadGenerator = (RoadGenerator)EditorGUILayout.ObjectField(
                "Road Generator",
                worldGenerator.roadGenerator,
                typeof(RoadGenerator),
                true);

            worldGenerator.buildingGenerator = (BuildingGenerator)EditorGUILayout.ObjectField(
                "Building Generator",
                worldGenerator.buildingGenerator,
                typeof(BuildingGenerator),
                true);
        }

        showBaseSettings = EditorGUILayout.Foldout(showBaseSettings, "Base Settings");
        if (showBaseSettings)
        {
            worldGenerator.terrainGeneratorSeed = EditorGUILayout.IntField(
                "Terrain Generator Seed",
                worldGenerator.terrainGeneratorSeed);

            worldGenerator.roadGeneratorSeed = EditorGUILayout.IntField(
                "Road Generator Seed",
                worldGenerator.roadGeneratorSeed);

            worldGenerator.buildingGeneratorSeed = EditorGUILayout.IntField(
                "Building Generator Seed",
                worldGenerator.buildingGeneratorSeed);

            worldGenerator.autoUpdate = EditorGUILayout.Toggle(
                "Auto Update",
                worldGenerator.autoUpdate);
        }

        GUILayout.Space(10);

        int newTab = GUILayout.Toolbar(tabIndex, tabs, GUILayout.Height(30));
        if (newTab != tabIndex)
        {
            tabIndex = newTab;
            EditorPrefs.SetInt(TAB_KEY, tabIndex);
        }

        GUILayout.Space(10);

        var changed = false;

        switch (tabIndex)
        {
            case 0:
                changed = DrawSection(ref terrainEditor, worldGenerator.terrainGenerator);

                break;
            case 1:
                changed = DrawSection(ref waterEditor, worldGenerator.waterGenerator);

                break;
            case 2:
                changed = DrawSection(ref roadEditor, worldGenerator.roadGenerator);

                break;
            case 3:
                changed = DrawSection(ref buildingEditor, worldGenerator.buildingGenerator);

                break;
        }

        if (changed && worldGenerator.autoUpdate)
        {
            RegenerateCurrentStage(worldGenerator);
        }

        DrawButtons(worldGenerator);
    }

    private bool DrawSection<T>(ref Editor cachedEditor, T targetObj) where T : Object
    {
        if (!targetObj)
        {
            return false;
        }

        if (!cachedEditor)
        {
            cachedEditor = CreateEditor(targetObj);
        }

        EditorGUI.BeginChangeCheck();
        cachedEditor.OnInspectorGUI();
        bool changed = EditorGUI.EndChangeCheck();

        if (changed)
        {
            EditorUtility.SetDirty(targetObj);
        }

        return changed;
    }

    private void DrawSectionAll(WorldGenerator worldGenerator)
    {
        if (GUILayout.Button("Clear all"))
        {
            worldGenerator.ClearAll();
        }
    }

    private void DrawButtons(WorldGenerator worldGenerator)
    {
        EditorGUILayout.Space(5);

        switch (tabIndex)
        {
            case 0:
                DrawTerrainButtons(worldGenerator);
                break;
            case 1:
                DrawWaterButtons(worldGenerator);
                break;
            case 2:
                DrawRoadButtons(worldGenerator);
                break;
            case 3:
                DrawBuildingButtons(worldGenerator);
                break;
        }
    }

    private void DrawTerrainButtons(WorldGenerator worldGenerator)
    {
        if (GUILayout.Button("Build Terrain"))
        {
            worldGenerator.InitializeAndGenerateTerrain();
        }

        if (GUILayout.Button("Clear All"))
        {
            worldGenerator.ClearAll();
        }
    }

    private void DrawWaterButtons(WorldGenerator worldGenerator) { }

    private void DrawRoadButtons(WorldGenerator worldGenerator)
    {
        if (GUILayout.Button("Build Roads"))
        {
            worldGenerator.InitializeAndGenerateRoads();
        }

        if (GUILayout.Button("Clear All"))
        {
            worldGenerator.ClearAll();
        }
    }

    private void DrawBuildingButtons(WorldGenerator worldGenerator)
    {
        if (GUILayout.Button("Build Buildings"))
        {
            worldGenerator.InitializeAndGenerateBuildings();
        }
    }

    private void RegenerateCurrentStage(WorldGenerator worldGenerator)
    {
        switch (tabIndex)
        {
            case 0:
                worldGenerator.InitializeAndGenerateTerrain();
                break;
            case 2:
                worldGenerator.InitializeAndGenerateRoads();
                break;
            case 3:
                worldGenerator.InitializeAndGenerateBuildings();
                break;
        }
    }

    private void DestroyEditor(ref Editor editor)
    {
        if (!editor)
        {
            return;
        }

        DestroyImmediate(editor);
        editor = null;
    }
}