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

    public Transform bodyTransform;

    public float tweenDecal = 1f;
    public float tweenDuration = 0.2f;
    public float quickTweenDuration = 0.2f;

    private Transform _transform;

    private Transform GetTransform
    {
        get
        {
            if (_transform == null)
            {
                _transform = GetComponent<Transform>();
            }

            return _transform;
        }
    }

	private float timer = 0f;

	private int throwDirection = 1;

	private float settleDuration = 0.5f;

	public Transform anchor;
	public bool thrown = false;

	public int targetResult = 1;

    MeshRenderer[] rends;

    // Use this for initialization
    public void Init () {

		settleDuration = DiceManager.Instance.settlingDuration;

        rends = GetComponentsInChildren<MeshRenderer>();
	}

	void LateUpdate () {
		if (thrown) {
			float x = Random.Range (0f,360f);
			float y = Random.Range (0f,360f);
			float z = Random.Range (0f,360f);

			bodyTransform.rotation = Quaternion.Euler ( new Vector3(x,y,z) );
		}
	}

	public void Reset () {

		// POS
		throwDirection = DiceManager.Instance.ThrowDirection;
		Vector3 pos = anchor.position;
		pos.x *= throwDirection;
		GetTransform.position = pos;

        bodyTransform.localPosition = Vector3.zero;
        bodyTransform.rotation = Quaternion.identity;
        bodyTransform.localScale = Vector3.one;

		// SCALE
		GetTransform.localScale = Vector3.one;

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

        Vector3 p = GetTransform.position + dir * Random.Range(minForce, maxForce);
        GetTransform.DOMove(p, DiceManager.Instance.throwDuration);

	}

	#region settle
	float targetScale;

	public void SettleDown ()
    {

        bodyTransform.DOScale(0f, tweenDuration).SetEase(Ease.InBounce);

        Color c = rends[0].material.color;

        c.a = 0f;

		foreach (MeshRenderer rend in rends) {
            rend.material.DOColor(c, settleDuration);
		}

	}

	public void SettleUp() {

        //GetTransform.DOKill();
        bodyTransform.DOScale(Vector3.one * 1.2f, settleDuration);
        
        //Tween.Bounce (transform);

		/*foreach (SpriteRenderer rend in GetComponentsInChildren<SpriteRenderer>()) {

			HOTween.To (rend, settleDuration, "color", Color.white);

		}*/
	}
	#endregion

	#region properties
	public int result {
		get {
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

        thrown = false;

        Vector3 rot = Vector3.zero;

        switch (i) {
		case 1:
                rot = new Vector3(0,0,0);
			//GetTransform.up = Vector3.up;
			break;
		case 2:
                rot = new Vector3(0,0,90);
                //GetTransform.right = Vector3.up;
                break;
		case 3:
                rot = new Vector3(0, 0, 90);
                //GetTransform.forward = Vector3.up;
                break;
		case 4:
                rot = new Vector3(-270, -180, 90);
                //GetTransform.forward= -Vector3.up;
                break;
		case 5:
                rot = new Vector3(-180, -180, 90);
                //GetTransform.right = -Vector3.up;
                break;
		case 6:
                rot = new Vector3(-180, -180, 0);
                //GetTransform.up = -Vector3.up;
                break;
		}

        Vector3 init = Vector3.zero;
        Vector3 p = Vector3.up * tweenDecal;

        bodyTransform.DOLocalMove(p, tweenDuration);
        bodyTransform.DOLocalMove(init, quickTweenDuration).SetDelay(tweenDuration);
        bodyTransform.DORotate(rot, tweenDuration + quickTweenDuration);
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

        bodyTransform.DOScale(0f, tweenDuration).SetEase(Ease.InBounce);

        /*foreach (var rend in rends)
        {
            rend.material.DOFade(0f, settleDuration);
        }*/
    }
    #endregion
}
