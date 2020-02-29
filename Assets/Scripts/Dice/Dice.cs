using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class Dice : MonoBehaviour {
	
	Vector3[] directions = new Vector3[6] {
		// 4
		-Vector3.forward,
		// 6
		Vector3.down,
		// 5
		Vector3.left,
		// 2
		Vector3.right,
		// 3
		Vector3.forward,
		// 1
		Vector3.back,

	};

	[SerializeField] private float minForce = 500f;
	[SerializeField] private float minTorque = 100f;
	[SerializeField] private float maxForce = 700f;
	[SerializeField] private float maxTorque = 200f;

	private float timer = 0f;

	private int throwDirection = 1;

	private float settleDuration = 0.5f;

	public Transform anchor;
	private bool thrown = false;

	public int targetResult = 1;

    SpriteRenderer[] rends;

	// Use this for initialization
	public void Init () {
		settleDuration = DiceManager.Instance.settlingDuration;

        rends = GetComponentsInChildren<SpriteRenderer>();
	}

	void LateUpdate () {
		if (thrown) {
			float x = Random.Range (0f,360f);
			float y = Random.Range (0f,360f);
			float z = Random.Range (0f,360f);

			transform.rotation = Quaternion.Euler ( new Vector3(x,y,z) );
		}
	}

	public void Reset () {

		// POS
		throwDirection = DiceManager.Instance.ThrowDirection;
		Vector3 pos = anchor.transform.position;
		pos.x *= throwDirection;
		transform.position = pos;

		// SCALE
		transform.localScale = Vector3.one;

	}

	public void Throw () {

		if (DiceManager.Instance.outcome < 1)
			targetResult = Random.Range (1, 7);
		else
			targetResult = DiceManager.Instance.outcome;

		thrown = true;

		//EnablePhysics ();

		Vector3 dir = Vector3.right;
        //GetComponent<Rigidbody> ().AddForce ( dir * throwDirection *  Random.Range (minForce , maxForce) );

        Vector3 p = transform.position + dir * Random.Range(minForce, maxForce);
        transform.DOMove(p, DiceManager.Instance.throwDuration);

	}

	#region settle
	float targetScale;

	public void SettleDown () {

        transform.DOKill();
        transform.DOScale(Vector3.one * 0.8f, settleDuration);

        Color c = rends[0].color;

        c.a = 0.6f;

		foreach (SpriteRenderer rend in rends) {
            rend.DOColor(c, settleDuration);
		}

	}

	public void SettleUp() {

        transform.DOKill();
        transform.DOScale(Vector3.one * 1.2f, settleDuration);
        
        //Tween.Bounce (transform);

		/*foreach (SpriteRenderer rend in GetComponentsInChildren<SpriteRenderer>()) {

			HOTween.To (rend, settleDuration, "color", Color.white);

		}*/
	}
	#endregion

	#region properties
	public int result {
		get {
//			Vector3[] dirs = new Vector3[6] {
//				transform.up,
//				transform.right,
//				transform.forward,
//				-transform.forward,
//				-transform.right,
//				-transform.up,
//			};
//
//			int i = 1;
//
//			foreach ( Vector3 d in dirs ) {
//				if (Vector3.Dot (d, Vector3.up) > 0.5f) {
//					
//					return i;
//				}
//
//				++i;
//			}
//
//			Debug.LogError ("Coudn't find value of die\nreturning 0");
//			return 0;
			return targetResult;
		}
	}


	public int ThrowDirection {
		get {
			return throwDirection;
		}
		set {
			throwDirection = value;
		}
	}
	public void TurnToDirection (int i ) {
		
//		GetComponent<Rigidbody> ().velocity = Vector3.zero;
//		GetComponent<Rigidbody> ().angularVelocity = Vector3.zero;

		thrown = false;

		switch(i) {
		case 1:
			transform.up = Vector3.up;
			break;
		case 2:
			transform.right = Vector3.up;
			break;
		case 3:
			transform.forward = Vector3.up;
			break;
		case 4:
			transform.forward= -Vector3.up;
			break;
		case 5:
			transform.right = -Vector3.up;
			break;
		case 6:
			transform.up = -Vector3.up;
			break;
		}
	}
	#endregion

	#region dice color
	DiceTypes currType;
	public void Paint ( DiceTypes type ) {

		currType = type;

		foreach ( SpriteRenderer rend in GetComponentsInChildren<SpriteRenderer>() ) {
			rend.color = DiceManager.Instance.DiceColors (type);
		}
	}
	public void Fade () {
		
	}
	#endregion
}
