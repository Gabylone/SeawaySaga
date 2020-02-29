using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class textWrapper : MonoBehaviour {

	Text text;

	public string initText = "Bonjour je m'appelle Gabriel Sarnikov je mange huit poulets et 23 bouts de cacas merci bonsoir";

	public int startIndex = 15;

	// Use this for initialization
	void Start () {
		text = GetComponent<Text> ();
	}
	
	// Update is called once per frame
	void Update () {
		text.text = FitText (initText);
	}

	string FitText (string str)
	{
		int currStartIndex = startIndex;

		if (currStartIndex >= str.Length)
			return str;

		int spaceIndex = str.IndexOf (" ", currStartIndex);

		while (spaceIndex >= startIndex) {

			print (" space index : " + spaceIndex + " / start index : " + currStartIndex);

			str = str.Insert (spaceIndex, "\n");

			currStartIndex += startIndex;

			if (currStartIndex >= str.Length) {
				print ("out of string");
				break;
			}

			spaceIndex = str.IndexOf (" ", currStartIndex);

			//			print (spaceIndex);

			if (startIndex >= 100) {
				print ("hors de la limite");
				break;
			}
		}

		return str;
	}
}
