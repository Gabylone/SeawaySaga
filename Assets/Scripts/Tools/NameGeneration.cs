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
			text = text.Replace ( "CAPITAINE" , Crews.playerCrew.captain.MemberName );
		}

		if ( text.Contains ("OTHERNAME") ) {
			text = text.Replace ( "OTHERNAME" , Crews.enemyCrew.captain.MemberName );
		}

		if ( text.Contains ("NOMBATEAU") ) {
			text = text.Replace ( "NOMBATEAU" , Boats.playerBoatInfo.Name);
		}

		if ( text.Contains ("DIRECTIONTOFORMULA") ) {
			text = text.Replace ( "DIRECTIONTOFORMULA" , FormulaManager.Instance.getDirectionToFormula () );
		}

        if (text.Contains("NOMTRESOR"))
        {
            text = text.Replace("NOMTRESOR", MapGenerator.mapParameters.mapName);
        }

        if ( text.Contains ("BOUNTY") ) {
			text = text.Replace ( "BOUNTY" , Karma.Instance.bounty.ToString () );
		}

		if ( text.Contains ("FORMULA") ) {

			Formula formula = FormulaManager.Instance.formulas.Find(x=>x.coords == Boats.playerBoatInfo.coords);

			if ( formula.found == false ) {
				if ( onDiscoverFormula != null ) {
					onDiscoverFormula(formula);
				}
			}

			formula.found = true;

			text = text.Replace ( "FORMULA" , formula.name.ToUpper() );
		}

		if ( text.Contains ("RANDOMFEMALENAME") ) {
			text = text.Replace ( "RANDOMFEMALENAME" , CrewCreator.Instance.femaleNames[Random.Range (0,CrewCreator.Instance.femaleNames.Length)]);
		}

		if ( text.Contains ("RANDOMMALENAME") ) {
			text = text.Replace ( "RANDOMMALENAME" , CrewCreator.Instance.maleNames[Random.Range (0,CrewCreator.Instance.maleNames.Length)]);
		}

		return text;
	}
}
