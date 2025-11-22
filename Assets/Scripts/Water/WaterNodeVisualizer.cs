using UnityEditor;
using UnityEngine;

[ExecuteAlways]
public class WaterNodeVisualizer : MonoBehaviour
{
    public WaterGenerator waterGenerator;

    public Vector3 verticalOffset;

    private void OnDrawGizmos()
    {
        if (waterGenerator == null)
        {
            return;
        }

        if (waterGenerator.waterBodies == null || waterGenerator.waterBodies.Count == 0)
        {
            return;
        }

        foreach (WaterBody body in waterGenerator.waterBodies)
        {
            if (body == null || body.nodes == null || body.nodes.Count == 0)
            {
                continue;
            }

            Gizmos.color = Color.cyan;

            foreach (WaterNode node in body.nodes)
            {
                if (!node)
                {
                    continue;
                }

                Gizmos.DrawSphere(node.transform.position + verticalOffset, 50f);
            }

            Gizmos.color = Color.blue;

            Color lineColor = Color.blue;

            for (var i = 0; i < body.nodes.Count - 1; i++)
            {
                if (body.nodes[i] == null || body.nodes[i + 1] == null)
                {
                    continue;
                }

                Vector3 p1        = body.nodes[i].transform.position + verticalOffset;
                Vector3 p2        = body.nodes[i + 1].transform.position + verticalOffset;
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