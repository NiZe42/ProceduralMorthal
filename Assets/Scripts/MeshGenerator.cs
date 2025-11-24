using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator
{
    public static MeshData GenerateTerrainMesh(
        float[,] heightMap,
        float heightMultiplier,
        AnimationCurve heightCurve,
        out float[,] meshHeightMap)
    {
        int width  = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        meshHeightMap = new float[width, height];

        float topLeftX = (width - 1.0f) / -2.0f;
        float topLeftZ = (width - 1.0f) / 2.0f;

        var meshData    = new MeshData(width, height);
        var vertexIndex = 0;

        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                meshHeightMap[x, y] = heightMap[x, y] * heightMultiplier *
                    heightCurve.Evaluate(heightMap[x, y]);

                meshData.vertices[vertexIndex] = new Vector3(
                    topLeftX + x,
                    meshHeightMap[x, y],
                    topLeftZ - y);

                meshData.uvs[vertexIndex] = new Vector2(x / (float)width, y / (float)height);

                if (x < width - 1.0f && y < height - 1.0f)
                {
                    meshData.AddTriangle(vertexIndex, vertexIndex + width, vertexIndex + width + 1);
                    meshData.AddTriangle(vertexIndex + width + 1, vertexIndex + 1, vertexIndex);
                }

                vertexIndex++;
            }
        }

        return meshData;
    }

    public static (float minHeight, float maxHeight) GetMinMax(Vector3[] vertices)
    {
        var minHeight = float.MaxValue;
        var maxHeight = float.MinValue;

        foreach (Vector3 vertice in vertices)
        {
            if (vertice.y < minHeight)
            {
                minHeight = vertice.y;
            }

            if (vertice.y > maxHeight)
            {
                maxHeight = vertice.y;
            }
        }

        return(minHeight, maxHeight);
    }

    public static MeshData GenerateRiverMeshData(
        List<Vector3> splinePoints,
        float riverWidth,
        float waterSurfaceHeight)
    {
        if (splinePoints == null || splinePoints.Count < 2)
        {
            return null;
        }

        int coreVertCount  = splinePoints.Count * 2;
        var extraVertCount = 4;
        int vertexCount    = coreVertCount + extraVertCount;

        int triCount = (splinePoints.Count - 1) * 2 * 3 + 2 * 2 * 3;

        var meshData = new MeshData(vertexCount, triCount);

        var totalLength = 0f;
        var lengths     = new List<float>();

        for (var i = 0; i < splinePoints.Count - 1; i++)
        {
            float d = Vector3.Distance(splinePoints[i], splinePoints[i + 1]);
            lengths.Add(d);
            totalLength += d;
        }

        var uvOffset = 0f;
        var vIndex   = 0;

        float averageSegmentLength = totalLength / splinePoints.Count;

        float tileLength = averageSegmentLength;

        float tileWidth = riverWidth / 5f;

        for (var i = 0; i < splinePoints.Count; i++)
        {
            var pos = new Vector3(splinePoints[i].x, waterSurfaceHeight, splinePoints[i].z);

            Vector3 dir = i == splinePoints.Count - 1
                ? (splinePoints[i] - splinePoints[i - 1]).normalized
                : (splinePoints[i + 1] - splinePoints[i]).normalized;

            Vector3 cross = Vector3.Cross(Vector3.up, dir).normalized;

            Vector3 left  = pos - cross * (riverWidth * 0.5f);
            Vector3 right = pos + cross * (riverWidth * 0.5f);

            meshData.vertices[vIndex]     = left;
            meshData.vertices[vIndex + 1] = right;

            float v = uvOffset / tileLength;

            float uLeft  = 0f / tileWidth;
            float uRight = riverWidth / tileWidth;

            meshData.uvs[vIndex]     = new Vector2(uLeft, v);
            meshData.uvs[vIndex + 1] = new Vector2(uRight, v);

            vIndex += 2;

            if (i < lengths.Count)
            {
                uvOffset += lengths[i];
            }
        }

        Vector3 dirStart = (splinePoints[1] - splinePoints[0]).normalized;
        float   extend   = riverWidth * 0.6f;

        int extraStartLeft  = coreVertCount;
        int extraStartRight = coreVertCount + 1;

        Vector3 left0  = meshData.vertices[0];
        Vector3 right0 = meshData.vertices[1];

        Vector3 startLeftExt  = left0 - dirStart * extend;
        Vector3 startRightExt = right0 - dirStart * extend;

        meshData.vertices[extraStartLeft]  = startLeftExt;
        meshData.vertices[extraStartRight] = startRightExt;

        float startV = -extend / tileLength;
        meshData.uvs[extraStartLeft]  = new Vector2(0f / tileWidth, startV);
        meshData.uvs[extraStartRight] = new Vector2(riverWidth / tileWidth, startV);

        Vector3 dirEnd = (splinePoints[^1] - splinePoints[^2]).normalized;

        int extraEndLeft  = coreVertCount + 2;
        int extraEndRight = coreVertCount + 3;

        int iLeftEnd  = coreVertCount - 2;
        int iRightEnd = coreVertCount - 1;

        Vector3 leftEnd  = meshData.vertices[iLeftEnd];
        Vector3 rightEnd = meshData.vertices[iRightEnd];

        Vector3 endLeftExt  = leftEnd + dirEnd * extend;
        Vector3 endRightExt = rightEnd + dirEnd * extend;

        meshData.vertices[extraEndLeft]  = endLeftExt;
        meshData.vertices[extraEndRight] = endRightExt;

        float endV = uvOffset / tileLength + extend / tileLength;
        meshData.uvs[extraEndLeft]  = new Vector2(0f / tileWidth, endV);
        meshData.uvs[extraEndRight] = new Vector2(riverWidth / tileWidth, endV);

        for (var i = 0; i < splinePoints.Count - 1; i++)
        {
            int a = i * 2;
            int b = a + 1;
            int c = a + 2;
            int d = a + 3;

            meshData.AddTriangle(a, c, b);
            meshData.AddTriangle(b, c, d);
        }

        meshData.AddTriangle(extraStartLeft, 0, extraStartRight);
        meshData.AddTriangle(extraStartRight, 0, 1);

        meshData.AddTriangle(iLeftEnd, extraEndLeft, iRightEnd);
        meshData.AddTriangle(iRightEnd, extraEndLeft, extraEndRight);

        return meshData;
    }

    public static MeshData GenerateLakeMeshData(
        List<Vector3> circlePoints,
        Vector3 center,
        float waterSurfaceHeight)
    {
        int ringCount   = circlePoints.Count;
        int vertexCount = ringCount + 1;
        int triCount    = ringCount * 3;

        var meshData = new MeshData(vertexCount, triCount);

        meshData.vertices[0] = new Vector3(
            circlePoints[0].x,
            waterSurfaceHeight,
            circlePoints[0].z);

        meshData.uvs[0] = new Vector2(0.5f, 0.5f);

        for (var i = 0; i < ringCount; i++)
        {
            meshData.vertices[i + 1] = new Vector3(
                circlePoints[i].x,
                waterSurfaceHeight,
                circlePoints[i].z);

            meshData.uvs[i + 1] = new Vector2(
                0.5f + (circlePoints[i].x - center.x) / (2f * circlePoints.Count),
                0.5f + (circlePoints[i].z - center.z) / (2f * circlePoints.Count));
        }

        for (var i = 0; i < ringCount - 1; i++)
        {
            meshData.AddTriangle(0, i + 2, i + 1);
        }

        meshData.AddTriangle(0, 1, ringCount);

        return meshData;
    }
}

public class MeshData
{
    private int triangleIndex;
    public int[] triangles;
    public Vector2[] uvs;
    public Vector3[] vertices;

    public MeshData(int width, int height)
    {
        vertices  = new Vector3[width * height];
        triangles = new int[(width - 1) * (height - 1) * 6];
        uvs       = new Vector2[width * height];
    }

    public void AddTriangle(int a, int b, int c)
    {
        triangles[triangleIndex]     =  a;
        triangles[triangleIndex + 1] =  b;
        triangles[triangleIndex + 2] =  c;
        triangleIndex                += 3;
    }

    public Mesh CreateMesh()
    {
        var mesh = new Mesh
        {
            vertices  = vertices,
            triangles = triangles,
            uv        = uvs
        };

        mesh.RecalculateNormals();
        return mesh;
    }
}