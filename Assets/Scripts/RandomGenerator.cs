using UnityEditor;
using UnityEngine;
using Random = System.Random;

public static class RandomGenerator
{
    private const string EditorSeedKey = "GlobalProceduralSeed";
    private static Random random;

    public static void Initialize(int seed)
    {
        if (seed == 0)
        {
            seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        }

        random = new Random(seed);

#if UNITY_EDITOR
        EditorPrefs.SetInt(EditorSeedKey, seed);
#endif

        Debug.Log("[RandomGenerator] Initialized with seed: " + seed);
    }

    public static int Next()
    {
        return random.Next();
    }

    public static int Next(int max)
    {
        return random.Next(max);
    }

    public static int Next(int min, int max)
    {
        return random.Next(min, max);
    }

    public static float NextFloat()
    {
        return(float)random.NextDouble();
    }

    public static float NextFloat(float min, float max)
    {
        return min + (float)random.NextDouble() * (max - min);
    }
}