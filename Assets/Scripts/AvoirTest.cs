using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvoirTest : MonoBehaviour {

    public Transform target;

    public float minDistance = 0f;
    public float maxDistance = 10f;

    public float speed = 10f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {


        float distanceToTarget = Vector3.Distance( target.position , transform.position );

        Vector3 dirToTarget = target.position - transform.position;

        transform.forward = dirToTarget;

        if (distanceToTarget < minDistance)
        {
            float angle = (maxDistance - distanceToTarget) * 90f / minDistance;


            transform.Rotate(Vector3.up * angle);


        }

        transform.Translate(transform.forward * speed * Time.deltaTime);

    }
}


