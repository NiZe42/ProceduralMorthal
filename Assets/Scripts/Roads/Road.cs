using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Road : MonoBehaviour
{
    public GameObject plankPrefab;

    public GameObject supportPrefab;

    public float buildHeight;
    public int supportEvery;
    public float doublePlankPercentageProbability;

    public float staircaseStepHeight;
    public float staircaseStepLength;

    public List<RoadNode> nodes = new List<RoadNode>();
    public Transform plankContainer;

    public bool isLeftRoad;

    public void Generate()
    {
        if (nodes.Count < 2)
        {
            Debug.LogError("Road requires at least 2 RoadNodes.");
            return;
        }

        ClearPlanks();
        CreateContainer();
        GenerateStaircases();
        GeneratePlanks();
    }

    public void Clear()
    {
        if (nodes.Count == 0)
        {
            return;
        }

        foreach (RoadNode node in nodes)
        {
            if (node != null)
            {
                DestroyImmediate(node.gameObject);
            }
        }

        nodes.Clear();
    }

    private void CreateContainer()
    {
        if (plankContainer != null)
        {
            DestroyImmediate(plankContainer.gameObject);
        }

        plankContainer = new GameObject("PlankContainer").transform;
        plankContainer.SetParent(transform);
        plankContainer.localPosition = Vector3.zero;
        plankContainer.localRotation = Quaternion.identity;
    }

    private void ClearPlanks()
    {
        if (plankContainer != null)
        {
            DestroyImmediate(plankContainer.gameObject);
        }
    }

    private void GenerateStaircases()
    {
        GenerateEndStaircase(
            this,
            staircaseStepHeight,
            staircaseStepLength,
            WorldGenerator.Instance.waterGenerator.waterSurfaceHeight);

        GenerateStartStaircase(
            this,
            staircaseStepHeight,
            staircaseStepLength,
            WorldGenerator.Instance.waterGenerator.waterSurfaceHeight);
    }

    public void GenerateEndStaircase(
        Road road,
        float stepHeight,
        float stepLength,
        float waterHeight)
    {
        Vector3 start = road.nodes[^2].transform.position;
        Vector3 end   = road.nodes[^1].transform.position;
        Vector3 dir   = (end - start).normalized;

        Vector3 stepPos       = end;
        float   currentHeight = buildHeight;

        var maxSteps  = 20;
        var stepIndex = 0;

        while (stepIndex < maxSteps)
        {
            Vector3 nextPos       = stepPos + dir * stepLength;
            float   terrainHeight = SampleTerrainHeight(nextPos);

            if (terrainHeight >= nextPos.y + stepHeight)
            {
                break;
            }

            nextPos.y = currentHeight + stepHeight;

            Instantiate(
                    plankPrefab,
                    nextPos,
                    Quaternion.LookRotation(dir, Vector3.up) * Quaternion.Euler(0f, 90f, 0f))
                .transform
                .SetParent(plankContainer.transform);

            stepPos       =  nextPos;
            currentHeight += stepHeight;
            stepIndex++;
        }
    }

    public void GenerateStartStaircase(
        Road road,
        float stepHeight,
        float stepLength,
        float waterHeight)
    {
        Vector3 start = road.nodes[1].transform.position;
        Vector3 end   = road.nodes[0].transform.position;
        Vector3 dir   = (end - start).normalized;

        Vector3 stepPos       = end;
        float   currentHeight = buildHeight;

        var maxSteps  = 20;
        var stepIndex = 0;

        while (stepIndex < maxSteps)
        {
            Vector3 nextPos       = stepPos + dir * stepLength;
            float   terrainHeight = SampleTerrainHeight(nextPos);

            if (terrainHeight >= nextPos.y + stepHeight)
            {
                break;
            }

            nextPos.y = currentHeight + stepHeight;

            Instantiate(
                    plankPrefab,
                    nextPos,
                    Quaternion.LookRotation(dir, Vector3.up) * Quaternion.Euler(0f, 90f, 0f))
                .transform
                .SetParent(plankContainer.transform);

            stepPos       =  nextPos;
            currentHeight += stepHeight;
            stepIndex++;
        }
    }

    private float SampleTerrainHeight(Vector3 pos)
    {
        RaycastHit hit;
        if (Physics.Raycast(
            pos + Vector3.up * 50f,
            Vector3.down,
            out hit,
            100f,
            LayerMask.GetMask("Terrain")))
        {
            return hit.point.y;
        }

        return pos.y;
    }

    private void GeneratePlanks()
    {
        if (nodes.Count < 2)
        {
            return;
        }

        List<Vector3> pts    = nodes.Select(n => n.transform.position).ToList();
        var           spline = new RoadSpline(pts);

        float realPlankLength;
        float realPlankWidth;

        {
            GameObject sample = plankPrefab != null ? plankPrefab : supportPrefab;
            var        mf     = sample.GetComponentInChildren<MeshFilter>();

            Vector3 size = mf.sharedMesh.bounds.size;
            size            = Vector3.Scale(size, mf.transform.lossyScale);
            realPlankLength = Mathf.Abs(size.z);
            realPlankWidth  = Mathf.Abs(size.x);
        }

        var   globalIndex = 0;
        var   distance    = 0f;
        float step        = realPlankWidth;

        float totalLength = EstimateSplineLength(spline);

        while (distance < totalLength)
        {
            float t = distance / totalLength;

            Vector3 pos = spline.GetPoint(t);
            pos.y = buildHeight;

            Vector3 dir = spline.GetTangent(t);
            Quaternion rotation = Quaternion.LookRotation(dir, Vector3.up) *
                Quaternion.Euler(0f, 90f, 0f);

            bool useSupport = globalIndex % supportEvery == 0;
            if (globalIndex % supportEvery == 1)
            {
                useSupport =
                    WorldGenerator.Instance.roadRandomGenerator.NextPercentage(
                        doublePlankPercentageProbability);
            }

            GameObject prefab = useSupport ? supportPrefab : plankPrefab;

            Instantiate(
                prefab,
                pos,
                rotation,
                plankContainer);

            globalIndex++;
            distance += step;
        }
    }

    private float EstimateSplineLength(RoadSpline spline)
    {
        var       length  = 0f;
        const int samples = 200;

        Vector3 previousPoint = spline.GetPoint(0f);
        for (var i = 1; i <= samples; i++)
        {
            float   t = i / (float)samples;
            Vector3 p = spline.GetPoint(t);
            length        += Vector3.Distance(previousPoint, p);
            previousPoint =  p;
        }

        return length;
    }
}