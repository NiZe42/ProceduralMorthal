using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    [SerializeField]
    private int seed;

    [SerializeField]
    private TerrainGenerator terrainGenerator;

    public void InitializeAndGenerateTerrain()
    {
        RandomGenerator.Initialize(seed);
        GenerateTerrain();
    }

    private void GenerateTerrain()
    {
        terrainGenerator.GenerateTerrain();
    }

    public void InitializeAndGenerateWaterBanks()
    {
        RandomGenerator.Initialize(seed);
        GenerateWaterBanks();
    }

    private void GenerateWaterBanks() { }

    public void InitializeAndGenerateAll()
    {
        RandomGenerator.Initialize(seed);
        GenerateAll();
    }

    private void GenerateAll()
    {
        GenerateTerrain();
    }
}