using System.Collections.Generic;
using UnityEngine;

public class BuildingGenerator : MonoBehaviour
{
    public GameObject buildingPrefab;

    public float minDistanceAlongRoad;
    public float buildingOffset;

    public List<Building> generatedBuildings = new List<Building>();

    public void GenerateBuildings()
    {
        RoadGenerator roadGenerator = WorldGenerator.Instance.roadGenerator;
        ClearBuildings();

        if (!roadGenerator || !buildingPrefab)
        {
            return;
        }

        foreach (Road road in roadGenerator.roads)
        {
            if (road.nodes.Count < 2)
            {
                continue;
            }

            TryPlaceAlongRoad(road);
        }
    }

    private void TryPlaceAlongRoad(Road road)
    {
        for (var i = 0; i < road.nodes.Count - 1; i++)
        {
            Vector3 start = road.nodes[i].transform.position;
            Vector3 end   = road.nodes[i + 1].transform.position;

            Vector3 roadDir = (end - start).normalized;

            Vector3 placementDir = road.isLeftRoad
                ? Quaternion.Euler(0f, -90f, 0f) * roadDir
                : Quaternion.Euler(0f, 90f, 0f) * roadDir;

            float segmentLength = Vector3.Distance(start, end);

            var   distanceAlong             = 0f;
            float distanceSinceLastBuilding = Mathf.Infinity;

            while (distanceAlong < segmentLength)
            {
                Vector3 onRoadPos    = start + roadDir * distanceAlong;
                Vector3 candidatePos = onRoadPos + placementDir * buildingOffset;

                if (distanceSinceLastBuilding < minDistanceAlongRoad)
                {
                    distanceAlong             += minDistanceAlongRoad * 0.5f;
                    distanceSinceLastBuilding += minDistanceAlongRoad * 0.5f;
                    continue;
                }

                if (CanPlaceBuilding(candidatePos, placementDir))
                {
                    Quaternion rot = Quaternion.LookRotation(placementDir) *
                        Quaternion.Euler(0f, 180f, 0f);

                    Building building = PlaceBuilding(candidatePos, rot, road);

                    var   col    = building.GetComponent<BoxCollider>();
                    float stride = col ? col.bounds.size.z : minDistanceAlongRoad;

                    distanceAlong             += stride;
                    distanceSinceLastBuilding =  0f;
                }
                else
                {
                    float step = minDistanceAlongRoad * 0.5f;
                    distanceAlong             += step;
                    distanceSinceLastBuilding += step;
                }
            }
        }
    }

    private bool CanPlaceBuilding(Vector3 position, Vector3 forwardDir)
    {
        var prefabCol = buildingPrefab.GetComponent<BoxCollider>();
        if (!prefabCol)
        {
            return false;
        }

        Vector3 halfExtents = prefabCol.size * 0.5f;
        halfExtents *= 1.05f;

        Quaternion rot = Quaternion.LookRotation(forwardDir, Vector3.up);
        Vector3 worldHalfExtents = Vector3.Scale(halfExtents, buildingPrefab.transform.lossyScale);

        Collider[] hits = Physics.OverlapBox(
            position + rot * prefabCol.center,
            worldHalfExtents,
            rot);

        foreach (Collider c in hits)
        {
            if (c.transform.GetComponent<Building>())
            {
                return false;
            }
        }

        return true;
    }

    private Building PlaceBuilding(Vector3 position, Quaternion forwardDir, Road road)
    {
        GameObject obj = Instantiate(
            buildingPrefab,
            position,
            forwardDir,
            transform);

        var building = obj.GetComponent<Building>();
        if (building)
        {
            building.Generate(position, forwardDir);
            generatedBuildings.Add(building);
        }

        return building;
    }

    public void ClearBuildings()
    {
        foreach (Building building in generatedBuildings)
        {
            if (building)
            {
                DestroyImmediate(building.gameObject);
            }
        }

        generatedBuildings.Clear();
    }

    private float SampleTerrain(Vector3 pos)
    {
        if (Physics.Raycast(
            pos + Vector3.up * 200f,
            Vector3.down,
            out RaycastHit hit,
            400f))
        {
            return hit.point.y;
        }

        return pos.y;
    }
}