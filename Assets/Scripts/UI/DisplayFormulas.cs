using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayFormulas : MonoBehaviour {

	public Image[] images;
	public Text[] uiTexts;

    bool visible = false;

    public Transform buttonTransform;

    public GameObject group;

    private void Start()
    {
        HideFormulas();
    }

    public void SwitchVisible()
    {
        visible = !visible;

        if ( visible)
        {
            ShowFormulas();
        }
        else
        {
            HideFormulas();
        }

        Tween.Bounce(buttonTransform);
    }

	public void ShowFormulas () {

        group.SetActive(true);

        foreach (var item in images)
        {
            item.gameObject.SetActive(false);
        }

        bool foundOne = false;

		int formulaIndex = 0;
		foreach (var form in FormulaManager.Instance.formulas) {

			if (form.found == true) {
				
				images [formulaIndex].gameObject.SetActive (true);
				uiTexts [formulaIndex].text = form.name.ToUpper();

				foundOne = true;

			}

			formulaIndex++;

		}

		if ( foundOne == false ) {
			images [0].gameObject.SetActive (true);
			uiTexts [0].text = "Pas d'indice";
		}

	}

    public void HideFormulas()
    {
        group.SetActive(false);
    }
}
