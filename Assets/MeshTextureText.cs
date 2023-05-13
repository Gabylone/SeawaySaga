using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshTextureText : MonoBehaviour
{
    public int width = 28;

    public int height = 20;

    public int x = 0;
    public int y = 0;

    public Renderer rend;

    public int mip = 0;
    public float mipLevel = 0f;

    private void Start()
    {
        Texture2D texture = new Texture2D(width, height);
        texture.Reinitialize(width, height);
        texture.filterMode = FilterMode.Point;

        texture.mipMapBias = mipLevel;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                texture.SetPixel(x, y, Color.white);
            }
        }

        texture.Apply();

        rend.material.mainTexture = texture;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.B))
        {
            Texture2D texture = rend.material.mainTexture as Texture2D;


            int x = Random.Range(0, width);
            int y = Random.Range(0, height);

        texture.SetPixel(x, y, Color.red, mip);

            texture.filterMode = FilterMode.Point;
            texture.Apply();

            rend.material.mainTexture = texture;
        }
    }
}
