using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WaterBody : MonoBehaviour
{
    public List<WaterNode> nodes = new List<WaterNode>();

    public MeshCollider meshCollider;
    public MeshFilter riverMeshFilter;
    public MeshRenderer riverMeshRenderer;

    public void Clear()
    {
        if (nodes.Count == 0)
        {
            return;
        }

        foreach (WaterNode node in nodes)
        {
            DestroyImmediate(node.gameObject);
        }

        nodes.Clear();
    }
}