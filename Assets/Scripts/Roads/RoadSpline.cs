using System.Collections.Generic;
using UnityEngine;

public class RoadSpline
{
    private readonly List<Vector3> points;

    public RoadSpline(List<Vector3> pts)
    {
        points = pts;
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

    public Vector3 GetPoint(float t)
    {
        int count = points.Count;

        if (count < 2)
        {
            return points[0];
        }

        float scaled = t * (count - 1);
        int   i      = Mathf.FloorToInt(scaled);
        float localT = scaled - i;

        int i0 = Mathf.Clamp(i - 1, 0, count - 1);
        int i1 = Mathf.Clamp(i, 0, count - 1);
        int i2 = Mathf.Clamp(i + 1, 0, count - 1);
        int i3 = Mathf.Clamp(i + 2, 0, count - 1);

        return CatmullRom(
            points[i0],
            points[i1],
            points[i2],
            points[i3],
            localT);
    }

    public Vector3 GetTangent(float t)
    {
        const float eps = 0.001f;
        Vector3     a   = GetPoint(Mathf.Clamp01(t - eps));
        Vector3     b   = GetPoint(Mathf.Clamp01(t + eps));
        return(b - a).normalized;
    }
}