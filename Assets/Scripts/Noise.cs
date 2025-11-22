using UnityEngine;

public static class Noise
{
    public static float[,] GenerateNoizeMap(
        RandomGenerator randomGenerator,
        int width,
        int height,
        float scale,
        int octaves,
        float persistence,
        float lacunarity)
    {
        if (Mathf.Abs(scale) < .0001f)
        {
            scale = .0001f;
        }

        var noiseMap = new float[width, height];

        var maxNoiseHeight = float.MinValue;
        var minNoiseHeight = float.MaxValue;

        float halfWidth  = width / 2;
        float halfHeight = height / 2;

        var octaveOffsets = new Vector2[octaves];
        for (var i = 0; i < octaves; i++)
        {
            float offsetX = randomGenerator.NextFloat(-100000f, 100000f);
            float offsetY = randomGenerator.NextFloat(-100000f, 100000f);
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                float amplitude   = 1;
                float frequency   = 1;
                float noizeHeight = 0;

                for (var i = 0; i < octaves; i++)
                {
                    float sampleX = (x - halfWidth) / scale * frequency + octaveOffsets[i].x;
                    float sampleY = (y - halfHeight) / scale * frequency + octaveOffsets[i].y;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;

                    noizeHeight += perlinValue * amplitude;
                    amplitude   *= persistence;
                    frequency   *= lacunarity;
                }

                if (noizeHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noizeHeight;
                }

                if (noizeHeight < minNoiseHeight)
                {
                    minNoiseHeight = noizeHeight;
                }

                noiseMap[x, y] = noizeHeight;
            }
        }

        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
            }
        }

        return noiseMap;
    }
}