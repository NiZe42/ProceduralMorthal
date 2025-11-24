using UnityEditor;
using UnityEngine;

public class TerrainDisplay : MonoBehaviour
{
    [SerializeField]
    private Renderer targetRenderer;

    [SerializeField]
    private MeshFilter targetMeshFilter;

    [SerializeField]
    private MeshRenderer targetMeshRenderer;

    [SerializeField]
    private MeshCollider targetMeshCollider;

    public void DrawTexture(Texture2D texture)
    {
        targetRenderer.sharedMaterial.mainTexture = texture;
        targetRenderer.transform.localScale = new Vector3(texture.width, 1.0f, texture.height);
    }

    // A Hack to make it the same size as the terrain plane.
    public void DrawMesh(MeshData meshData, Texture2D texture, Vector3 scale)
    {
        Mesh                   mesh   = meshData.CreateMesh();
        (float min, float max) minMax = MeshGenerator.GetMinMax(mesh.vertices);
        Material               mat    = targetMeshRenderer.sharedMaterial;

#if UNITY_EDITOR
        EditorApplication.delayCall += () =>
        {
            if (this == null || mat == null)
            {
                return;
            }

            mat.SetVector(
                "_Tiling",
                new Vector2(texture.width * scale.x, texture.height * scale.y));

            mat.SetFloat("_minHeight", minMax.min);
            mat.SetFloat("_maxHeight", minMax.max);
        };
#endif
        if (Application.isPlaying)
        {
            mat.SetVector(
                "_Tiling",
                new Vector2(texture.width * scale.x, texture.height * scale.y));

            mat.SetFloat("_minHeight", minMax.min);
            mat.SetFloat("_maxHeight", minMax.max);
        }

        targetMeshFilter.sharedMesh                   = mesh;
        targetMeshRenderer.sharedMaterial.mainTexture = texture;
        targetMeshCollider.sharedMesh                 = mesh;
        targetMeshFilter.transform.localScale         = scale;
    }

    // Expected to only manipulate with vertices inside of the mesh, but not outside
    public void ChangeMesh(Mesh mesh)
    {
        targetMeshFilter.sharedMesh   = mesh;
        targetMeshCollider.sharedMesh = mesh;

        // (float min, float max) minMax = MeshGenerator.GetMinMax(mesh.vertices);
        // Material               mat    = targetMeshRenderer.sharedMaterial;

        // mat.SetFloat("_minHeight", minMax.min);
        // mat.SetFloat("_maxHeight", minMax.max);
    }

    public void ClearTerrain()
    {
        targetRenderer.sharedMaterial.mainTexture = null;
    }

    public void ClearMesh()
    {
        targetMeshFilter.sharedMesh                   = null;
        targetMeshRenderer.sharedMaterial.mainTexture = null;
    }
}