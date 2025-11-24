using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class Building : MonoBehaviour
{
    public GameObject basePrefab;
    public GameObject floorPrefab;
    public GameObject twoFloorPrefab;

    public GameObject sideRoofLeftPrefab;
    public GameObject sideRoofRightPrefab;
    public GameObject forwardRoofSmallPrefab;
    public GameObject forwardRoofBigPrefab;

    public GameObject plankPrefab;
    public Transform plankContainer;

    public float twoFloorRightProbability = 0.25f;
    public float sideRoofLeftProbability = 0.5f;
    public float sideRoomRightProbability = 0.5f;
    public float forwardRoofSmallProbability = 0.5f;

    public float frontOffset = 1f;

    public int staircaseCount;
    public int staircaseStepLength;
    public int staircaseStepHeight;

    public void Generate(Vector3 position, Quaternion forwardDir)
    {
        RandomGenerator rng = WorldGenerator.Instance.buildingRangomGenerator;

        transform.position = position;
        transform.rotation = forwardDir;

        GameObject baseObj = Instantiate(
            basePrefab,
            transform.position - 2f * Vector3.up,
            transform.rotation,
            transform);

        bool       spawnTwoFloor = rng.NextFloat() < twoFloorRightProbability && twoFloorPrefab;
        GameObject floorObj      = null;

        if (spawnTwoFloor)
        {
            Vector3 twoFloorPos = transform.position;
            floorObj = Instantiate(
                twoFloorPrefab,
                twoFloorPos,
                transform.rotation,
                transform);
        }
        else
        {
            if (floorPrefab)
            {
                Vector3 floorPos = transform.position;
                floorObj = Instantiate(
                    floorPrefab,
                    floorPos,
                    transform.rotation,
                    transform);
            }
        }

        if (sideRoofLeftPrefab != null && rng.NextFloat() < sideRoofLeftProbability)
        {
            Vector3 leftPos = transform.position + transform.right * -5.5f;

            Instantiate(
                sideRoofLeftPrefab,
                leftPos,
                transform.rotation * Quaternion.Euler(0f, 90f, 180f),
                transform);
        }

        if (sideRoofRightPrefab != null && rng.NextFloat() < sideRoomRightProbability)
        {
            Vector3 rightPos = transform.position + transform.right * 5.5f;

            Instantiate(
                sideRoofRightPrefab,
                rightPos,
                transform.rotation,
                transform);
        }

        if (rng.NextFloat() < forwardRoofSmallProbability)
        {
            if (forwardRoofSmallPrefab != null)
            {
                Vector3 ofs = transform.forward * 3.5f;
                Vector3 pos = transform.position + ofs;
                Instantiate(
                    forwardRoofSmallPrefab,
                    pos,
                    transform.rotation,
                    transform);
            }
        }
        else
        {
            if (forwardRoofBigPrefab != null)
            {
                Vector3 ofs = transform.right * 0.2f + transform.forward * 3.5f;
                Vector3 pos = transform.position + ofs;
                Instantiate(
                    forwardRoofBigPrefab,
                    pos,
                    transform.rotation,
                    transform);
            }
        }

        // GeneratePlanks(transform.position, forwardDir);

        // GenerateStaircaseSimple(transform.position, forwardDir);
    }

    public void GeneratePlanks(Vector3 position, Vector3 forwardDir)
    {
        if (!plankPrefab || !plankContainer)
        {
            return;
        }

        Vector3 basePos = transform.position + forwardDir * frontOffset - 2 * Vector3.down;

        Quaternion plankRot = Quaternion.LookRotation(forwardDir, Vector3.up) *
            Quaternion.Euler(0f, 90f, 0f);

        var    meshRenderer = plankPrefab.GetComponent<MeshRenderer>();
        Bounds bounds = meshRenderer ? meshRenderer.bounds : new Bounds(Vector3.zero, Vector3.one);

        float plankLength = bounds.size.z;
        float plankWidth  = bounds.size.x;

        Vector3 rowCenter = basePos - forwardDir * (plankLength + 0.1f);

        float leftExtra  = sideRoofLeftPrefab ? 3f + plankWidth * 0.5f : 0f;
        float rightExtra = sideRoofRightPrefab ? 3f + plankWidth * 0.5f : 0f;

        float halfWidth  = plankWidth * 0.5f;
        float totalLeft  = halfWidth + leftExtra;
        float totalRight = halfWidth + rightExtra;

        float totalWidth = totalLeft + totalRight;

        Vector3 leftStart = rowCenter - transform.right * totalLeft;

        int plankCount = Mathf.CeilToInt(totalWidth / plankWidth);

        for (var i = 0; i < plankCount; i++)
        {
            Vector3 pos = leftStart + transform.right * (i * plankWidth);

            Instantiate(
                plankPrefab,
                pos,
                plankRot,
                plankContainer);
        }
    }

    public void GenerateStaircaseSimple(Vector3 startPos, Vector3 forwardDir)
    {
        if (plankPrefab == null || plankContainer == null)
        {
            return;
        }

        Quaternion plankRot = Quaternion.LookRotation(forwardDir, Vector3.up) *
            Quaternion.Euler(0f, 90f, 0f);

        Vector3 current = startPos + forwardDir * (frontOffset + 0.1f); // start slightly ahead
        for (var i = 0; i < staircaseCount; i++)
        {
            current   += forwardDir * staircaseStepLength;
            current.y += staircaseStepHeight;

            Instantiate(
                plankPrefab,
                current,
                plankRot,
                plankContainer);
        }
    }
}