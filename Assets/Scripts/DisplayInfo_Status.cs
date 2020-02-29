using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayInfo_Status : DisplayInfo {

	public override void Start ()
	{
		base.Start ();

		StatusFeedback.onTouchStatusFeedback += HandleOnTouchStatusFeedback;
	}

	void HandleOnTouchStatusFeedback (Fighter.Status status)
	{
		string str = "";

		switch (status) {
		case Fighter.Status.KnockedOut:
			str = "Assomé ! Le combattant ne jouera pas son prochain tour";
			break;
		case Fighter.Status.PreparingAttack:
			str = "Le combattant prépare une attaque";
			break;
		case Fighter.Status.Enraged:
			str = "Enragé : Le combattant gagne 10 points d'énergie par tour";
			break;
		case Fighter.Status.Jagged:
			str = "Un médecin s'est occupé du combattant, il gagne 10 points de vies par tour";
			break;
		case Fighter.Status.Poisonned:
			str = "Empoisonné, le combattant perd 10 points de vie par tour";
			break;
		case Fighter.Status.Provoking:
			str = "Les 3 prochaines attaques sont dirigées vers le combattant, mieux vaut bien le protéger";
			break;
		case Fighter.Status.Protected:
			str = "Le combattant est protégé, il ne reçoit que la moitié des dégats";
			break;
		case Fighter.Status.Toasted:
			str = "Après avoir trinqué avec un cuistôt, le combattant inflige 150% de dégats";
			break;
		case Fighter.Status.BearTrapped:
			str = "Le combattant a posé un piège, son prochain attaquant perd 15 points de vie";
			break;
		case Fighter.Status.Cussed:
			str = "Insulté et humilié, le combattant n'inflige que la moitié de ses dégats";
			break;
		case Fighter.Status.None:
			break;
		default:
			break;
		}

		confirmGroup.SetActive (true);

		Display (status.ToString (),str);

		Move (Corner.None);

	}
}
