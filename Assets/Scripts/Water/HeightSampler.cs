using UnityEngine;

public static class HeightSampler
{
    public static float GetHeightFromMeshHeightMap(
        Vector3 worldPos,
        float[,] nonNormalizedHeightMap,
        Vector3 meshWorldScale)
    {
        int width  = nonNormalizedHeightMap.GetLength(0);
        int height = nonNormalizedHeightMap.GetLength(1);

        float meshScale = meshWorldScale.x / (width - 1);

        float topLeftX = (width - 1.0f) / -2.0f;
        float topLeftZ = (width - 1.0f) / 2.0f;

        float x = (worldPos.x - topLeftX) / meshScale;
        float y = (topLeftZ - worldPos.z) / meshScale;

        int ix = Mathf.FloorToInt(x);
        int iy = Mathf.FloorToInt(y);

        if (ix < 0 || iy < 0 || ix >= width || iy >= height)
        {
            return 0f;
        }

        return nonNormalizedHeightMap[ix, iy];
    }
}