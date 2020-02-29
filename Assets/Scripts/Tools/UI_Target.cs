using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UI_Target : MonoBehaviour {

	RectTransform rectTransform;

	[SerializeField]
	private Transform target;

	// Use this for initialization
	void Start () {
		rectTransform = GetComponent<RectTransform> ();
	}
	
	// Update is called once per frame
	void Update () {

		rectTransform.position = target.position;
	}
}
