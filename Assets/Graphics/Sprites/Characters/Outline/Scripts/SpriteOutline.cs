using UnityEngine;

[ExecuteInEditMode]
public class SpriteOutline : MonoBehaviour
{
    public Color color = Color.white;

    [Range(0, 16)]
    public int outlineSize = 1;

    private SpriteRenderer spriteRenderer;

    float timer = 0f;

    private float duration = 0.7f;

    private bool reverse = false;

    private float minSize = 5f;
    private float maxSize = 5f;

    public float targetAlpha = 0.2f;

    void OnEnable()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        UpdateOutline(true);
    }

    void OnDisable()
    {
        UpdateOutline(false);
    }

    void Update()
    {
        if (reverse)
        {
            timer -= Time.deltaTime;

            if ( timer <= 0)
            {
                reverse = false;
            }
        }
        else
        {
            timer += Time.deltaTime;

            if (timer >= duration)
            {
                reverse = true;
            }
        }

        UpdateOutline(true);
    }

    void UpdateOutline(bool outline)
    {
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        spriteRenderer.GetPropertyBlock(mpb);
        mpb.SetFloat("_Outline", outline ? 1f : 0);

        Color clearColor = color;
        clearColor.a = targetAlpha;

        mpb.SetColor("_OutlineColor", Color.Lerp( clearColor , color , timer / duration ));

        mpb.SetFloat("_OutlineSize", Mathf.Lerp( minSize , maxSize , timer / duration ));

        spriteRenderer.SetPropertyBlock(mpb);
    }
}
