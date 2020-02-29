using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldFeedback : InfoFeedbacks {

	public override void Start ()
	{
		base.Start ();

		GoldManager.onCheckGold += HandleOnCheckGold;
		GoldManager.onChangeGold += HandleOnChangeGold;
	}

	void HandleOnChangeGold (int value)
	{
		if (value > 0) {
			Print (value.ToString(), Color.yellow);
		} else {
			Print (value.ToString(), Color.red);
		}
	}

	void HandleOnCheckGold (bool enoughtGold)
	{
		if (enoughtGold == false) {
//			Print ("Pas assez d'or", Color.red);
			Print ("Pas assez d'or", Color.red);
		}

	}

}
