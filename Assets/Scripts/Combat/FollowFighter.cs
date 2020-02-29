using UnityEngine;
using System.Collections;

public class FollowFighter : MonoBehaviour {

	[SerializeField]
	private Transform target;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 pos = transform.position;
		pos.x = target.position.x;

		transform.position = pos;
	}
}
