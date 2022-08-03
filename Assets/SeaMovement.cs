using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeaMovement : MonoBehaviour
{
    public Material material;
    Vector2 offset;
    public Vector2 dir;
    public float speed;
    public int id;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        offset += dir * speed * Time.deltaTime;
        material.SetTextureOffset("_MainTex", offset);
    }
}
