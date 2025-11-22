using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WaterGenerator : MonoBehaviour
{
    [SerializeField]
    private TerrainDisplay terrainDisplay;

    public MeshFilter targetMeshFilter;

    public GameObject waterBodyPrefab;

    public float waterHeight;
    public float waterSurfaceHeight;
    public float riverWidth;
    public float bankSmoothness;

    public int pointsPerWaterSegment;
    public int circleResolution;
    public LayerMask terrainMask;

    public List<WaterBody> waterBodies = new List<WaterBody>();

    public int selectedBodyIndex;

    [HideInInspector]
    public Mesh oldMesh;

    public void AddWaterBody()
    {
        GameObject waterBodyObject = Instantiate(waterBodyPrefab);
        var        waterBody       = waterBodyObject.GetComponent<WaterBody>();

        waterBodies.Add(waterBody);
        selectedBodyIndex = waterBodies.Count - 1;
    }

    public void RemoveWaterBody(int index)
    {
        if (index < 0 || index >= waterBodies.Count)
        {
            return;
        }

        waterBodies[index].Clear();
        DestroyImmediate(waterBodies[index].gameObject);
        waterBodies.RemoveAt(index);
        selectedBodyIndex = Mathf.Clamp(selectedBodyIndex, 0, waterBodies.Count - 1);
    }

    public void ClearAllWaterBodies()
    {
        for (int i = waterBodies.Count - 1; i >= 0; i--)
        {
            RemoveWaterBody(i);
        }

        selectedBodyIndex = -1;
    }

    public void AddNode(Vector3 position)
    {
        if (waterBodies.Count == 0)
        {
            AddWaterBody();
            if (selectedBodyIndex < 0)
            {
                selectedBodyIndex = 0;
            }
        }

        WaterBody body = waterBodies[selectedBodyIndex];

        var riverNodeObject = new GameObject("WaterNode");
        riverNodeObject.transform.position = position;
        riverNodeObject.transform.SetParent(body.transform);

        var node = riverNodeObject.AddComponent<WaterNode>();
        body.nodes.Add(node);

        Undo.RegisterCreatedObjectUndo(riverNodeObject, "Add WaterNode");
    }

    public void Carve()
    {
        oldMesh = targetMeshFilter.sharedMesh;
        if (targetMeshFilter == null)
        {
            Debug.LogWarning("No MeshFilter assigned!");
            return;
        }

        Mesh      mesh       = targetMeshFilter.sharedMesh;
        Vector3[] vertsLocal = mesh.vertices;
        var       vertsWorld = new Vector3[vertsLocal.Length];

        Transform t = targetMeshFilter.transform;

        for (var i = 0; i < vertsLocal.Length; i++)
        {
            vertsWorld[i] = t.TransformPoint(vertsLocal[i]);
        }

        foreach (WaterBody body in waterBodies)
        {
            if (body.nodes.Count == 0)
            {
                continue;
            }

            var curve = new List<Vector3>();

            if (body.nodes.Count == 1)
            {
                Vector3 center = body.nodes[0].transform.position;

                curve = GenerateCircleSpline(center);
                CarveCircle(ref vertsWorld, center);
                UpdateRiverMesh(body, curve);
                continue;
            }

            curve = GenerateSplinePoints(body.nodes);
            for (var i = 0; i < curve.Count - 1; i++)
            {
                CarveSegment(ref vertsWorld, curve[i], curve[i + 1]);
            }

            UpdateRiverMesh(body, curve);
        }

        for (var i = 0; i < vertsLocal.Length; i++)
        {
            vertsLocal[i] = t.InverseTransformPoint(vertsWorld[i]);
        }

        mesh.vertices = vertsLocal;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        terrainDisplay.ChangeMesh(mesh);

        Debug.Log("Water carving complete.");
    }

    public void ResetTerrain()
    {
        WorldGenerator.Instance.InitializeAndGenerateTerrain();
    }

    private void CarveCircle(ref Vector3[] verts, Vector3 center)
    {
        float radius = riverWidth * 0.5f;

        for (var i = 0; i < verts.Length; i++)
        {
            float dist = Vector3.Distance(verts[i], center);

            if (dist < radius)
            {
                float norm    = dist / radius;
                float falloff = Mathf.Pow(1 - norm, bankSmoothness);

                verts[i].y = Mathf.Lerp(verts[i].y, waterHeight, falloff);
            }
        }
    }

    private List<Vector3> GenerateCircleSpline(Vector3 center)
    {
        float radius = riverWidth * 0.5f;

        var points = new List<Vector3>();

        for (var i = 0; i <= circleResolution; i++)
        {
            float a = i / (float)circleResolution * Mathf.PI * 2f;

            points.Add(
                new Vector3(
                    center.x + Mathf.Cos(a) * radius,
                    center.y,
                    center.z + Mathf.Sin(a) * radius));
        }

        return points;
    }

    private List<Vector3> GenerateSplinePoints(List<WaterNode> nodes)
    {
        var result = new List<Vector3>();

        for (var i = 0; i < nodes.Count - 1; i++)
        {
            Vector3 p0 = i == 0 ? nodes[i].transform.position : nodes[i - 1].transform.position;
            Vector3 p1 = nodes[i].transform.position;
            Vector3 p2 = nodes[i + 1].transform.position;
            Vector3 p3 = i + 2 >= nodes.Count
                ? nodes[i + 1].transform.position
                : nodes[i + 2].transform.position;

            for (var point = 0; point < pointsPerWaterSegment; point++)
            {
                float t = point / (float)pointsPerWaterSegment;
                result.Add(
                    CatmullRom(
                        p0,
                        p1,
                        p2,
                        p3,
                        t));
            }
        }

        result.Add(nodes[nodes.Count - 1].transform.position);

        return result;
    }

    private void UpdateRiverMesh(WaterBody body, List<Vector3> spline)
    {
        MeshData data = body.nodes.Count == 1
            ? MeshGenerator.GenerateLakeMeshData(
                spline,
                body.nodes[0].transform.position,
                waterSurfaceHeight)
            : MeshGenerator.GenerateRiverMeshData(spline, riverWidth, waterSurfaceHeight);

        Mesh mesh = data.CreateMesh();
        body.riverMeshFilter.sharedMesh = mesh;
        body.meshCollider.sharedMesh    = mesh;
    }

    private Vector3 CatmullRom(
        Vector3 p0,
        Vector3 p1,
        Vector3 p2,
        Vector3 p3,
        float t)
    {
        return 0.5f * (2f * p1 + (-p0 + p2) * t + (2f * p0 - 5f * p1 + 4f * p2 - p3) * (t * t) +
            (-p0 + 3f * p1 - 3f * p2 + p3) * (t * t * t));
    }

    private void CarveSegment(ref Vector3[] verts, Vector3 start, Vector3 end)
    {
        Vector3 dir    = (end - start).normalized;
        float   segLen = Vector3.Distance(start, end);

        for (var i = 0; i < verts.Length; i++)
        {
            Vector3 v   = verts[i];
            Vector3 toV = v - start;

            float   t       = Mathf.Clamp(Vector3.Dot(toV, dir), 0f, segLen);
            Vector3 closest = start + dir * t;

            float dist = Vector3.Distance(v, closest);

            float halfWidth = riverWidth / 2;

            if (dist < halfWidth)
            {
                float norm    = dist / halfWidth;
                float falloff = Mathf.Pow(1 - norm, bankSmoothness * 2f);

                float newY = Mathf.Lerp(v.y, waterHeight, falloff);
                verts[i] = new Vector3(v.x, newY, v.z);
            }
        }
    }
}