using System;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public enum DrawMode
    {
        NoiseMap,
        ColorMap,
        Mesh
    }

    public DrawMode drawMode;

    [SerializeField]
    private TerrainDisplay terrainDisplay;

    public int mapAxis;
    public Vector3 mapScale;

    public float noiseScale;
    public int octaves;

    [Range(0, 1)]
    public float persistence;

    public float lacunarity;

    public AnimationCurve damperHeightCurve;

    public float mountainHeightMultiplier;
    public AnimationCurve mountainHeightCurve;

    public TerrainType[] regions;

    public float[,] finalNonNormalizedHeightMap;

    private void OnValidate()
    {
        if (mapAxis < 3)
        {
            mapAxis = 3;
        }

        mapScale.y = mapScale.x;
        mapScale.z = mapScale.x;
        if (lacunarity < 1)
        {
            lacunarity = 1;
        }

        if (octaves < 0)
        {
            octaves = 0;
        }

        if (octaves > 20)
        {
            octaves = 20;
        }

        if (mountainHeightMultiplier < 1.0f)
        {
            mountainHeightMultiplier = 1.0f;
        }
    }

    public void GenerateTerrain(RandomGenerator randomGenerator)
    {
        float[,] noiseMap = Noise.GenerateNoizeMap(
            randomGenerator,
            mapAxis,
            mapAxis,
            noiseScale,
            octaves,
            persistence,
            lacunarity);

        ApplyHeightCurve(ref noiseMap);

        var colorMap = new Color[mapAxis * mapAxis];
        for (var x = 0; x < mapAxis; x++)
        {
            for (var y = 0; y < mapAxis; y++)
            {
                float currentHeight = noiseMap[x, y];
                for (var i = 0; i < regions.Length; i++)
                {
                    if (!(currentHeight < regions[i].height))
                    {
                        continue;
                    }

                    colorMap[y * mapAxis + x] = regions[i].color;
                    break;
                }
            }
        }

        switch (drawMode)
        {
            case DrawMode.NoiseMap:
                terrainDisplay.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
                break;

            case DrawMode.ColorMap:
                terrainDisplay.DrawTexture(
                    TextureGenerator.TextureFromColorMap(colorMap, mapAxis, mapAxis));

                break;

            case DrawMode.Mesh:
                terrainDisplay.DrawMesh(
                    MeshGenerator.GenerateTerrainMesh(
                        noiseMap,
                        mountainHeightMultiplier,
                        mountainHeightCurve,
                        out finalNonNormalizedHeightMap),
                    TextureGenerator.TextureFromColorMap(colorMap, mapAxis, mapAxis),
                    mapScale);

                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void ApplyHeightCurve(ref float[,] noiseMap)
    {
        for (var x = 0; x < mapAxis; x++)
        {
            for (var y = 0; y < mapAxis; y++)
            {
                noiseMap[x, y] = damperHeightCurve.Evaluate(noiseMap[x, y]);
            }
        }
    }

    public void ClearTerrain()
    {
        terrainDisplay.ClearTerrain();
        terrainDisplay.ClearMesh();
        finalNonNormalizedHeightMap = null;
    }
}