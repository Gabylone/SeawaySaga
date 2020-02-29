using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeightFeedback : InfoFeedbacks {

	public override void Start ()
	{
		base.Start ();

		WeightManager.onTooMuchWeight += HandleOnTooMuchWeight;

		BoatUpgradeManager.onUpgradeBoat += HandleOnUpgradeBoat;
	}

	void HandleOnUpgradeBoat (BoatUpgradeManager.UpgradeType upgradeType)
	{
		if (upgradeType == BoatUpgradeManager.UpgradeType.Cargo) {
			Print ("+ " + BoatUpgradeManager.Instance.cargoAugmentation);
		}
	}

	void HandleOnTooMuchWeight ()
	{
		Print ("Cargo plein", Color.red);
	}
}
