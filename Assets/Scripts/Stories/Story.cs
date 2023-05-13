using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Story {

    public int id = 0;

	public int param = 0;

	public float rangeMin = 0f;
	public float rangeMax = 0f;

	public string 	dataName 			= "";
	public string 	displayName 		= "";

    public float 	freq 			= 0f;

	public List<List<string>> content 	= new List<List<string>>();
	public List<Node> nodes 			= new List<Node> ();

	public Story ()
	{

	}

	public Node GetNode (string str ) {

		Node node = nodes.Find (x => x.name == str);

		if ( node == null ) {
			//Debug.LogError ( "node " + str + " doesn't exist in story " + dataName );
		}

		return node;

	}
}