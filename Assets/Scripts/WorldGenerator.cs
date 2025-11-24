using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    public static WorldGenerator Instance;
    public int terrainGeneratorSeed;
    public int roadGeneratorSeed;
    public int buildingGeneratorSeed;

    public TerrainGenerator terrainGenerator;
    public WaterGenerator waterGenerator;
    public RoadGenerator roadGenerator;
    public BuildingGenerator buildingGenerator;

    public bool autoUpdate;
    public RandomGenerator buildingRangomGenerator = new RandomGenerator();
    public RandomGenerator roadRandomGenerator = new RandomGenerator();
    public RandomGenerator terrainRandomGenerator = new RandomGenerator();

    public void InitializeAndGenerateTerrain()
    {
        terrainRandomGenerator.Initialize(terrainGeneratorSeed);
        GenerateTerrain();
    }

    private void GenerateTerrain()
    {
        terrainGenerator.GenerateTerrain(terrainRandomGenerator);
    }

    public void InitializeAndGenerateRoads()
    {
        roadRandomGenerator.Initialize(roadGeneratorSeed);
        GenerateRoads();
    }

    private void GenerateRoads()
    {
        roadGenerator.GenerateRoads();
    }

    public void InitializeAndGenerateBuildings()
    {
        buildingRangomGenerator.Initialize(buildingGeneratorSeed);
        GenerateBuildings();
    }

    private void GenerateBuildings()
    {
        buildingGenerator.GenerateBuildings();
    }

    public void ClearAll()
    {
        terrainGenerator.ClearTerrain();
        waterGenerator.ClearAllWaterBodies();
        roadGenerator.ClearAllRoads();
    }
}