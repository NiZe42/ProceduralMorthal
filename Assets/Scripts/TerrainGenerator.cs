using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    [SerializeField]
    private TerrainDisplay terrainDisplay;

    public int mapWidth;
    public int mapHeight;
    public float noiseScale;
    public int octaves;

    [Range(0, 1)]
    public float persistence;

    public float lacunarity;

    private void OnValidate()
    {
        if (mapWidth < 1)
        {
            mapWidth = 1;
        }

        if (mapHeight < 1)
        {
            mapHeight = 1;
        }

        if (lacunarity < 1)
        {
            lacunarity = 1;
        }

        if (octaves < 0)
        {
            octaves = 0;
        }
    }

    public void GenerateTerrain()
    {
        float[,] noizeMap = Noise.GenerateNoizeMap(
            mapWidth,
            mapHeight,
            noiseScale,
            octaves,
            persistence,
            lacunarity);

        terrainDisplay.DrawNoizeMap(noizeMap);
    }
}