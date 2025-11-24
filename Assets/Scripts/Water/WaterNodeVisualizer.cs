using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

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

        if (waterGenerator.debugWaterNodes)
        {
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

                    Gizmos.DrawSphere(node.transform.position + verticalOffset, 1f);
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

        if (waterGenerator.debugRiverSplines)
        {
            foreach (WaterBody body in waterGenerator.waterBodies)
            {
                if (body == null || body.nodes == null || body.nodes.Count == 0)
                {
                    continue;
                }

                Gizmos.color = Color.magenta;

                var splines = new List<Vector3>();
                if (body.nodes.Count != 1)
                {
                    splines = waterGenerator.GenerateSplinePoints(body.nodes);
                }

                foreach (Vector3 node in splines)
                {
                    Gizmos.DrawSphere(node, 1f);
                }

                Color lineColor = Color.magenta;

                for (var i = 0; i < splines.Count - 1; i++)
                {
                    Vector3 p1        = splines[i];
                    Vector3 p2        = splines[i + 1];
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