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
    public void DrawMesh(MeshData meshData, Texture2D texture)
    {
        Mesh mesh = meshData.CreateMesh();
        targetMeshFilter.sharedMesh                   = mesh;
        targetMeshRenderer.sharedMaterial.mainTexture = texture;
        targetMeshCollider.sharedMesh                 = mesh;
        targetMeshFilter.transform.localScale = new Vector3(
            texture.width / 10.0f,
            (texture.width + texture.height) / 20.0f,
            texture.height / 10.0f);
    }

    public void ChangeMesh(Mesh mesh)
    {
        targetMeshFilter.sharedMesh   = mesh;
        targetMeshCollider.sharedMesh = mesh;
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