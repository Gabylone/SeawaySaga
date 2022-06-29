using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scrollview_FAde : MonoBehaviour
{
    public Image image;

    public ScrollRect scrollRect;
    public float fade_buffer = 0.1f;

    public bool up = false;

    public Color initColor;

    public float fade_speed = 5f;

    public float debug;
    public bool fade;

    private void Start()
    {
        initColor = image.color;
    }

    // Update is called once per frame
    void Update()
    {
        debug = scrollRect.verticalNormalizedPosition;

        fade = up ?
            scrollRect.verticalNormalizedPosition > fade_buffer
            :
            scrollRect.verticalNormalizedPosition < fade_buffer;

        Color c = fade ? Color.clear : initColor;
        image.color = Color.Lerp( image.color , c , fade_speed * Time.deltaTime );
    }
}
