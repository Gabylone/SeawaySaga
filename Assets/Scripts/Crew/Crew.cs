using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct CrewParams {

	public int level;

	public int amount;

	public bool overideGenre;
	public bool male;

}

public class Crew {

	public bool hostile = false;
	List<Member> memberIDs = new List<Member>();

	public int row = 0;
	public int col = 0;


	public Crew () {

	}

	public Crew (CrewParams crewParams, int r , int c) {

		row = r;
		col = c;

		if (crewParams.amount == 0) {

			int amount = Random.Range ( Crews.playerCrew.CrewMembers.Count , 4 );

			if (Random.value < 0.6f)
				amount = Crews.playerCrew.CrewMembers.Count;
			
//			Debug.Log ("");
			crewParams.amount = Mathf.Clamp (amount, 1, 4);
		}

		for (int i = 0; i < crewParams.amount; ++i) {
			Member id = new Member (crewParams);
			if (crewParams.overideGenre) {
                if (crewParams.male)
                {
                    id.SetCharacterID(ApparenceType.genre, 0);
                }
                else
                {
                    id.SetCharacterID(ApparenceType.genre, 1);
                }
            }

			memberIDs.Add (id);


		}

	}

	public void Add ( Member id ) {
		memberIDs.Add (id);
	}

	public void Remove ( Member id ) {
		memberIDs.Remove (id);
	}

	public List<Member> MemberIDs {
		get {
			return memberIDs;
		}
	}
}