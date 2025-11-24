using UnityEditor;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

[ExecuteAlways]
public class RoadVisualizer : MonoBehaviour
{
    public RoadGenerator roadGenerator;

    public Vector3 verticalOffset;

    private void OnDrawGizmos()
    {
        if (roadGenerator == null)
        {
            return;
        }

        if (roadGenerator.roads == null || roadGenerator.roads.Count == 0)
        {
            return;
        }

        if (roadGenerator.debugRoads)
        {
            foreach (Road road in roadGenerator.roads)
            {
                if (road == null || road.nodes == null || road.nodes.Count == 0)
                {
                    continue;
                }

                Gizmos.color = Color.red;

                foreach (RoadNode node in road.nodes)
                {
                    if (!node)
                    {
                        continue;
                    }

                    Gizmos.DrawSphere(node.transform.position + verticalOffset, 1f);
                }

                Gizmos.color = Color.red;

                Color lineColor = Color.red;

                for (var i = 0; i < road.nodes.Count - 1; i++)
                {
                    if (road.nodes[i] == null || road.nodes[i + 1] == null)
                    {
                        continue;
                    }

                    Vector3 p1        = road.nodes[i].transform.position + verticalOffset;
                    Vector3 p2        = road.nodes[i + 1].transform.position + verticalOffset;
                    var     thickness = 5;
                    Handles.DrawBezier(
                        p1,
                        p2,
                        p1,
                        p2,
                        lineColor,
                        null,
                        thickness);
                }
            }
        }
    }
}