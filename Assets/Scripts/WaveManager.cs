using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance;

    public Wave[] waves;

    public float minimumX = 0f;
    public float minimumY = 0f;
    public float maximumX = 0f;
    public float maximumY = 0f;

    public float maxStartDelay = 2f;

    public float minSpeed = 0.2f;
    public float maxSpeed = 2.5f;

    public float duration = 2.5f;
    public float rate = 10f;

    private void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        //Vector3 dirToCam = (CamBehavior.Instance._transform.position- PlayerBoat.Instance.GetTransform.position).normalized;

        foreach (var item in waves)
        {
            if(item.move)
            {
                item.UpdateMovement();
                //item._transform.forward = -dirToCam;
            }
        }
    }

    public Vector3 GetRandomPos()
    {
        return PlayerBoat.Instance.transform.position + new Vector3(Random.Range(minimumX, maximumX), 0.39f, Random.Range(minimumY, maximumY));
    }

}
