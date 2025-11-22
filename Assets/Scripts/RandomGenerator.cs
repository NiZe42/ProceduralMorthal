using UnityEngine;
using Random = System.Random;

public class RandomGenerator
{
    private const string EditorSeedKey = "GlobalProceduralSeed";
    private static Random random;

    public void Initialize(int seed)
    {
        if (seed == 0)
        {
            seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        }

        random = new Random(seed);

        Debug.Log("[RandomGenerator] Initialized with seed: " + seed);
    }

    public int Next()
    {
        return random.Next();
    }

    public int Next(int max)
    {
        return random.Next(max);
    }

    public int Next(int min, int max)
    {
        return random.Next(min, max);
    }

    public float NextFloat()
    {
        return(float)random.NextDouble();
    }

    public float NextFloat(float min, float max)
    {
        return min + (float)random.NextDouble() * (max - min);
    }
}