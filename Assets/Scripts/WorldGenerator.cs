using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    public static WorldGenerator Instance;
    public int terrainGeneratorSeed;
    public int waterGeneratorSeed;

    public TerrainGenerator terrainGenerator;

    public WaterGenerator waterGenerator;

    public bool autoUpdate;

    private readonly RandomGenerator terrainRandomGenerator = new RandomGenerator();
    private readonly RandomGenerator waterRandomGenerator = new RandomGenerator();

    public void InitializeAndGenerateTerrain()
    {
        ClearAll();
        terrainRandomGenerator.Initialize(terrainGeneratorSeed);
        GenerateTerrain();
    }

    private void GenerateTerrain()
    {
        terrainGenerator.GenerateTerrain(terrainRandomGenerator);
    }

    public void InitializeAndGenerateWaterBanks()
    {
        ClearAll();
        waterRandomGenerator.Initialize(waterGeneratorSeed);
        GenerateWaterBanks();
    }

    private void GenerateWaterBanks() { }

    public void InitializeAndGenerateAll()
    {
        ClearAll();
        GenerateAll();
    }

    private void GenerateAll()
    {
        ClearAll();
        InitializeAndGenerateTerrain();
        InitializeAndGenerateWaterBanks();
    }

    public void ClearAll()
    {
        terrainGenerator.Clear();
    }
}