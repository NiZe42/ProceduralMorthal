using UnityEngine;

public class TerrainDisplay : MonoBehaviour
{
    [SerializeField]
    public Renderer targetRenderer;

    public void DrawNoizeMap(float[,] noizeMap)
    {
        int width  = noizeMap.GetLength(0);
        int height = noizeMap.GetLength(1);

        var noizeMapTexture = new Texture2D(
            width,
            height,
            TextureFormat.ARGB32,
            false);

        var colorMap = new Color[width * height];

        for (var x = 0; x < noizeMapTexture.width; x++)
        {
            for (var y = 0; y < noizeMapTexture.height; y++)
            {
                colorMap[y * width + x] = Color.Lerp(Color.black, Color.white, noizeMap[x, y]);
            }
        }

        noizeMapTexture.SetPixels(colorMap);
        noizeMapTexture.Apply();

        targetRenderer.sharedMaterial.mainTexture = noizeMapTexture;
        targetRenderer.transform.localScale = new Vector3(
            noizeMapTexture.width,
            1.0f,
            noizeMapTexture.height);
    }
}