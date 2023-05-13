using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteTest : MonoBehaviour
{
    public Image image;

    public int width = 28;

    public int height = 20;

    public int x = 0;
    public int y = 0;


    private void Start()
    {
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point;

        image.sprite = Sprite.Create(
               texture,
               new Rect(0, 0, width, height),
               Vector2.zero
               );

    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.B))
        {
            Texture2D texture = image.sprite.texture;

            texture.SetPixel(x, y, Color.red);
            texture.Apply();

           
        }
    }
}
