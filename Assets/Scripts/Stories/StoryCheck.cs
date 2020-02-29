using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryCheck : MonoBehaviour {

	void Update () {
		if ( Input.GetKeyDown(KeyCode.Insert) ) {
			StartCoroutine (CheckErrorInStories ());
		}
	}

	#region check nodes

	int decal = 0;
	int index = 0;

	string alphabet = "abcdefghijklmnopqrstuvwxyz";

	bool theresAnError = false;


	IEnumerator CheckErrorInStories ()
	{
		CheckStoryType (StoryLoader.Instance.getStories(StoryType.Normal));
		yield return new WaitForEndOfFrame ();
		CheckStoryType (StoryLoader.Instance.getStories(StoryType.Boat));
		yield return new WaitForEndOfFrame ();
		CheckStoryType (StoryLoader.Instance.getStories(StoryType.Clue));
		yield return new WaitForEndOfFrame ();
		CheckStoryType (StoryLoader.Instance.getStories(StoryType.Home));
		yield return new WaitForEndOfFrame ();
		CheckStoryType (StoryLoader.Instance.getStories(StoryType.Quest));
		yield return new WaitForEndOfFrame ();
		CheckStoryType (StoryLoader.Instance.getStories(StoryType.Treasure));

		if (!theresAnError) {
			Debug.Log ("Stories & Quests are perfect");
		}
	}

	Story storyToCheck;

	void CheckStoryType (List<Story> stories)
	{
		foreach (Story story in stories) {
			storyToCheck = story;
			CheckStory (story);
		}
	}

	void CheckStory ( Story story ) {

		decal = 0;

		foreach (List<string> contents in story.content) {

			CheckEveryCell (contents);

			++decal;

		}
	}

	void CheckEveryCell (List<string> contents)
	{
		index = 3;

		foreach (string content in contents) {

			CheckSingleCell (content);

			++index;

		}
	}

	void CheckSingleCell (string cellContent)
	{
		// check if empty
		if (cellContent.Length < 2)
			return;

		// check if node
		if ( cellContent[0] == '[' ) {
			return;
		}

		// check if choice
		if ( cellContent.Contains ("Choice") ) {
			return;
		}


		// CHECK FOR FUNCTION
		if (CellContainsFunction (cellContent) == false) {
			DisplayError ("Cell doesn't contain function",cellContent);
			return;
		}

		// CHECK NODE
		if (cellContent.Contains ("Node") ) {

			string nodeName = cellContent.Remove (0, 6);

			if ( LinkedToNode (nodeName) == false ) {

				DisplayError ("There's a NODE function, but the node has no link",cellContent);

				return;


			}
		}

		if (cellContent.Contains ("Switch") ) {

			string nodeName = cellContent.Remove (0, 8);

			if ( LinkedToNode (nodeName) == false ) {

				DisplayError ("There's a SWITCH function, but the node has no link",cellContent);

				return;


			}
		}

		if (cellContent.Contains ("ChangeStory") ) {
			CheckChangeStory (cellContent);
		}

		if (cellContent.Contains ("NewQuest") ) {
			CheckNewQuest (cellContent);
		}

		if (cellContent.Contains ("CheckQuest") ) {
			CheckNewQuest (cellContent);
		}

	}

	void CheckNewQuest (string cellContent)
	{
		if ( cellContent.Contains("CheckQuest") ){
			cellContent = cellContent.Remove (0, 12);
		} else {
			cellContent = cellContent.Remove (0, 10);
		}
		string[] parts = cellContent.Split (',');

		foreach ( string part in parts ) {
			if ( LinkedToNode(part) == false ) {
				DisplayError ("the node doesnt exist : " + part + " in",cellContent);
			}
		}
	}

	void CheckChangeStory (string cellContent) {
		// get second story name
		string storyName = cellContent.Remove (0, 13);
		storyName = storyName.Remove (storyName.IndexOf ('['));

		string[] nodes = cellContent.Remove (0, cellContent.IndexOf ('[') + 1).TrimEnd (']').Split ('/');

		if ( LinkedToNode (nodes[1]) == false ) {
			DisplayError ("the fallback node text : " + nodes[1] + " has no link",cellContent);
		}

		Story secondStory = StoryLoader.Instance.FindByName (storyName,StoryType.Normal);
		if ( secondStory == null ) {
			DisplayError ("Story " + storyName + " doesn't exist ",cellContent);
			return;
		}

		if ( LinkedToNode (nodes[0],secondStory) == false ) {
			DisplayError ("the target node has no link",cellContent);
		}
	}

	void DisplayError (string str,string content)
	{

		theresAnError = true;

		Debug.Log (storyToCheck.name);
		Debug.LogError (str);
		Debug.LogError ("CELL CONTENT : " + content);
		Debug.LogError ("ROW : " + alphabet [decal] + " / COLL " + index);
	}

	private bool LinkedToNode ( string nodeName ) {
		return LinkedToNode (nodeName, storyToCheck);
	}
	private bool LinkedToNode ( string nodeName , Story targetStory ) {

		nodeName = nodeName.TrimEnd ('\r', '\n');

		foreach (Node node in targetStory.nodes) {
			if (nodeName == node.name) {
				return true;
			}
		}

		return false;
	}

	private bool CellContainsFunction ( string str ) {

		foreach ( FunctionType func in System.Enum.GetValues(typeof(FunctionType)) ) {

			if (str.Contains (func.ToString())) {
				return true;
			}

		}

		return false;
	}
	#endregion
}
