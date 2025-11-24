using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoadGenerator : MonoBehaviour
{
    public float maxRiverRoadTurnAngle;
    public float riverRadiusMultiplier;
    public List<Road> roads = new List<Road>();

    public GameObject roadPrefab;

    public bool debugRoads;

    public void GenerateRoads()
    {
        ClearAllRoads();
        GenerateRiverRoads();
    }

    public void ClearAllRoads()
    {
        for (int i = roads.Count - 1; i >= 0; i--)
        {
            roads[i].Clear();
            DestroyImmediate(roads[i].gameObject);
        }

        roads.Clear();
    }

    private void GenerateRiverRoads()
    {
        WaterGenerator waterGenerator = WorldGenerator.Instance.waterGenerator;
        foreach (WaterBody body in waterGenerator.waterBodies)
        {
            if (!body || body.nodes.Count < 2)
            {
                continue;
            }

            /*List<Vector3> outer = ComputeOuterCurve(
                body.nodes.Select(node => node.transform.position).ToList());*/

            List<Road> riverRoads = GenerateRiverRoadsFromSpline(
                body.nodes.Select(node => node.transform.position).ToList());

            roads.AddRange(riverRoads);
        }

        foreach (Road road in roads)
        {
            Debug.Log("lala");
            road.Generate();
        }
    }

    private void GenerateOtherRoads() { }

    private Road CreateRoad()
    {
        GameObject roadObject = Instantiate(roadPrefab);
        var        road       = roadObject.GetComponent<Road>();
        return road;
    }

    private List<Vector3> ComputeOuterCurve(List<Vector3> spline)
    {
        float halfWidth = WorldGenerator.Instance.waterGenerator.riverWidth * 0.4f;

        var left  = new List<Vector3>();
        var right = new List<Vector3>();

        for (var i = 0; i < spline.Count; i++)
        {
            Vector3 pos = spline[i];
            Vector3 dir = i == spline.Count - 1
                ? spline[i] - spline[i - 1]
                : spline[i + 1] - spline[i];

            dir.Normalize();

            Vector3 cross = Vector3.Cross(Vector3.up, dir).normalized;

            left.Add(pos - cross * halfWidth);
            right.Add(pos + cross * halfWidth);
        }

        float leftLength  = SplineSquaredMagnitude(left);
        float rightLength = SplineSquaredMagnitude(right);

        return leftLength > rightLength ? left : right;
    }

    private RoadNode CreateNode(Vector3 position, Transform parent)
    {
        var nodeObject = new GameObject("RoadNode");
        nodeObject.transform.position = position;
        nodeObject.transform.SetParent(parent);
        var node = nodeObject.AddComponent<RoadNode>();
        return node;
    }

    private List<Road> GenerateRiverRoadsFromSpline(List<Vector3> spline)
    {
        var result = new List<Road>();
        if (spline == null || spline.Count < 2)
        {
            return result;
        }

        WaterGenerator waterGenerator = WorldGenerator.Instance.waterGenerator;
        float          halfWidth      = waterGenerator.riverWidth * 0.5f * riverRadiusMultiplier;

        var left  = new List<Vector3>();
        var right = new List<Vector3>();

        for (var i = 0; i < spline.Count; i++)
        {
            Vector3 pos = spline[i];
            Vector3 dir = i == spline.Count - 1
                ? spline[i] - spline[i - 1]
                : spline[i + 1] - spline[i];

            dir.Normalize();
            Vector3 cross = Vector3.Cross(Vector3.up, dir).normalized;

            left.Add(pos - cross * halfWidth);
            right.Add(pos + cross * halfWidth);
        }

        // simulate both starting conditions
        List<Road> scenarioLeft  = SimulateRoads(left, right, true);
        List<Road> scenarioRight = SimulateRoads(left, right, false);

        bool chosenLeft = scenarioLeft.Count <= scenarioRight.Count;
        if (chosenLeft)
        {
            foreach (Road road in scenarioRight)
            {
                road.Clear();
                DestroyImmediate(road.gameObject);
            }

            return scenarioLeft;
        }

        foreach (Road road in scenarioLeft)
        {
            road.Clear();
            DestroyImmediate(road.gameObject);
        }

        return scenarioRight;
    }

    private List<Road> SimulateRoads(List<Vector3> left, List<Vector3> right, bool startLeft)
    {
        var iLeft  = 0;
        var iRight = 0;

        bool useLeft = startLeft;

        var result = new List<Road>();

        while (iLeft < left.Count - 1 || iRight < right.Count - 1)
        {
            List<Vector3> side  = useLeft ? left : right;
            int           index = useLeft ? iLeft : iRight;

            int reach = ComputeRoadReach(side, index);
            if (reach == 0)
            {
                break;
            }

            int endIndex = Mathf.Min(index + reach, side.Count - 1);

            Road road = CreateRoad();
            road.isLeftRoad = useLeft;
            for (int j = index; j <= endIndex; j++)
            {
                RoadNode node = CreateNode(side[j], road.transform);
                road.nodes.Add(node);
            }

            result.Add(road);

            iLeft  = endIndex;
            iRight = endIndex;

            useLeft = !useLeft;
        }

        return result;
    }

    private int ComputeRoadReach(List<Vector3> side, int startIndex)
    {
        if (startIndex >= side.Count - 1)
        {
            return 0;
        }

        var     count   = 1;
        Vector3 prevDir = (side[startIndex + 1] - side[startIndex]).normalized;

        for (int i = startIndex + 2; i < side.Count; i++)
        {
            Vector3 newDir = (side[i] - side[i - 1]).normalized;
            float   angle  = Vector3.Angle(prevDir, newDir);

            if (angle > maxRiverRoadTurnAngle)
            {
                break;
            }

            prevDir = newDir;
            count++;
        }

        return count;
    }

    private float SplineSquaredMagnitude(List<Vector3> curve)
    {
        var sqrMagnitude = 0f;
        for (var i = 0; i < curve.Count - 1; i++)
        {
            sqrMagnitude += Vector3.SqrMagnitude(curve[i] - curve[i + 1]);
        }

        return sqrMagnitude;
    }
}