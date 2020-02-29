using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialMenuButton : MonoBehaviour {

	public Sprite disabledSprite;
	public Sprite enabledSprite;

	public Image image;
	public Image buttonImage;

    public Text tutoText;

	void Start () {

		UpdateButton ();

	}

	public void OnActivate () {

		KeepOnLoad.displayTuto = !KeepOnLoad.displayTuto;

		Tween.Bounce (transform, 0.2f , 1.05f);

		UpdateButton ();

		//
	}

	void UpdateButton() {

		if ( KeepOnLoad.displayTuto ) {

            tutoText.text = "Didactitiel : Activé";

			//image.sprite = enabledSprite;
			buttonImage.color = Color.green;
		} else {

            tutoText.text = "Didactitiel : Désactivé";

            //image.sprite = disabledSprite;
			buttonImage.color = Color.red;
		}

	}
}
