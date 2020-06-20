using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class NameGeneration : MonoBehaviour {

	public static NameGeneration Instance;

	void Awake () {
		Instance = this;

        onDiscoverFormula = null;
	}

    private char[] vowels = new char[6] {
		'a','e','y','u','i','o'
	};

	private char[] consumn = new char[20] {
		'z', 'r', 't', 'p', 'q', 's', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'm', 'w', 'x', 'c', 'v', 'b', 'n'
	};

	string syllab {
		get {
			return consumn[Random.Range(0,consumn.Length)].ToString () + vowels[Random.Range(0,vowels.Length)].ToString ();
		}
	}

	public string randomWord {
		get {

			string word = "";
			for (int i = 0; i < Random.Range (2, 4); ++i)
				word += syllab;
			return word;
		}
	}


	public delegate void OnDiscoverFormula(Formula Formula);
	public static OnDiscoverFormula onDiscoverFormula;

	public static string CheckForKeyWords ( string text ) {

		if ( text.Contains ("CAPITAINE") ) {
			text = text.Replace ( "CAPITAINE" , "<i>" + Crews.playerCrew.captain.MemberName + "</i>");

        }

		if ( text.Contains ("OTHERNAME") ) {
			text = text.Replace ("OTHERNAME", "<i>" + Crews.enemyCrew.captain.MemberName + "</i>");
        }

		if ( text.Contains ("NOMBATEAU") ) {
			text = text.Replace ("NOMBATEAU", "<i>" + Boats.Instance.playerBoatInfo.Name + "</i>");
        }

		if ( text.Contains ("DIRECTIONTOFORMULA") ) {
			text = text.Replace ("DIRECTIONTOFORMULA", "<b>" + FormulaManager.Instance.getDirectionToFormula() + "</b>");
        }

        if (text.Contains("NEXTTIME"))
        {
            if (TimeManager.Instance.dayState == TimeManager.DayState.Day)
            {
                text = text.Replace("NEXTTIME", "<i>" + "the night" + " </i>");

            }
            else
            {
                text = text.Replace("NEXTTIME", "<i>" + "the morning" + " </i>");
            }
        }

        if (text.Contains("NOMTRESOR"))
        {
            text = text.Replace("NOMTRESOR", "<i>" + MapGenerator.Instance.treasureName + " </i>");
        }

        if ( text.Contains ("BOUNTY") ) {
            text = text.Replace("BOUNTY", "<i>" + Karma.Instance.bounty.ToString() + " </i>");
        }

		if ( text.Contains ("FORMULA") ) {

			Formula formula = FormulaManager.Instance.formulas.Find(x=>x.coords == Boats.Instance.playerBoatInfo.coords);

			if ( formula.found == false ) {
				if ( onDiscoverFormula != null ) {
					onDiscoverFormula(formula);
				}
			}

			formula.found = true;

            text = text.Replace("FORMULA", "<i>" + formula.name.ToUpper() + " </i>");
        }

		if ( text.Contains ("RANDOMFEMALENAME") ) {
			text = text.Replace ( "RANDOMFEMALENAME" , CrewCreator.Instance.womanNames[Random.Range (0,CrewCreator.Instance.womanNames.Length)]);
		}

		if ( text.Contains ("RANDOMMALENAME") ) {
			text = text.Replace ( "RANDOMMALENAME" , CrewCreator.Instance.manNames[Random.Range (0,CrewCreator.Instance.manNames.Length)]);
		}

		return text;
	}
}
