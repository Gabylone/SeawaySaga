using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour {

	public int throwAmount = 6;

	public float percentOfThrowsContainingSix = 0f;

	int totalSixFound = 0;
	int totalThrows = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		bool containsSix = false;

		for (int currentNumberOfThrows = 0; currentNumberOfThrows < throwAmount; currentNumberOfThrows++) {

			int value = Random.Range ( 1,7 );

			if ( value == 6 ) {
				containsSix = true;
			}


		}

		totalThrows++;
		if (containsSix)
			totalSixFound++;

//		percentOfThrowsContainingSix = 100 - ( (float)totalSixFound / (float)totalThrows * 100 );
		percentOfThrowsContainingSix = 	( (float)totalSixFound / (float)totalThrows * 100 );

	
	}
}
