﻿using UnityEngine;
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

        if (crewParams.amount == 0)
        {
            int amount = Random.Range(Crews.playerCrew.CrewMembers.Count, 4);

            if (Random.value < 0.5f)
                amount = Crews.playerCrew.CrewMembers.Count;

            if ( MapGenerator.mapParameters.id == 0)
            {
                amount = Random.Range(1, 2);
            }

            //			Debug.Log ("");
            crewParams.amount = Mathf.Clamp(amount, 1, 4);

            if ( Crews.Instance.startJob != Job.None)
            {
                crewParams.amount = 1;
            }
        }

		for (int i = 0; i < crewParams.amount; ++i) {
			Member id = new Member (crewParams);

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