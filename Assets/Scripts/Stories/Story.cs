using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Story {

	public int param = 0;

	public float rangeMin = 0f;
	public float rangeMax = 0f;

	public string 	name 			= "";
	public float 	freq 			= 0f;

	public List<List<string>> content 	= new List<List<string>>();
	public List<Node> nodes 			= new List<Node> ();

	public Story ()
	{

	}

	public Story (
		string _name
	)
	{
		name = _name;
	}

	public Node GetNode (string str ) {

		Node node = nodes.Find (x => x.name == str);

		if ( node == null ) {
			Debug.LogError ( "node " + str + " doesn't exist in story " + name );
		}

		return node;

	}
}