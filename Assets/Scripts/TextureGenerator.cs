using UnityEngine;

public static class TextureGenerator
{
    public static Texture2D TextureFromColorMap(Color[] colourMap, int width, int height)
    {
        var texture = new Texture2D(width, height);
        texture.filterMode = FilterMode.Bilinear;
        texture.wrapMode   = TextureWrapMode.Clamp;
        texture.SetPixels(colourMap);
        texture.Apply();
        return texture;
    }

    public static Texture2D TextureFromHeightMap(float[,] heightMap)
    {
        int width  = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        var noiseMapTexture = new Texture2D(
            width,
            height,
            TextureFormat.ARGB32,
            false);

        var colorMap = new Color[width * height];

        for (var x = 0; x < noiseMapTexture.width; x++)
        {
            for (var y = 0; y < noiseMapTexture.height; y++)
            {
                colorMap[y * width + x] = Color.Lerp(Color.black, Color.white, heightMap[x, y]);
            }
        }

        noiseMapTexture.SetPixels(colorMap);
        noiseMapTexture.Apply();

        return noiseMapTexture;
    }
}