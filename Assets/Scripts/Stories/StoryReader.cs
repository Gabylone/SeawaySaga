using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class StoryReader : MonoBehaviour {

	public static StoryReader Instance;

	// the coords of the story being read.
	private int col = 0;
	private int row = 0;

	public int currentStoryLayer = 0;
	public int previousStoryLayer = 0;

	private bool waitToNextCell = false;
	private float timer = 0f;

	private StoryManager currentStoryManager;


	[SerializeField]
	private AudioClip pressInputButton;

	void Awake () {
		Instance = this;
	}

	void Start () {
		StoryInput.onPressInput += HandleOnPressInput;
		StoryFunctions.Instance.getFunction += HandleGetFunction;
	}

	void HandleOnPressInput ()
	{
		NextCell ();
		UpdateStory ();
	}

	void HandleGetFunction (FunctionType func, string cellParameters)
	{
		switch (func) {
		case FunctionType.ChangeStory:
			ChangeStory ();
			break;
		case FunctionType.Node:
			Node ();
			break;
		case FunctionType.Switch:
			Switch ();
			break;
		}
	}

	void Update () {
		if ( waitToNextCell )
			WaitForNextCell_Update ();
	}

	#region story flow
	public void Reset () {
		col = 0;
		row = 0;
	}
	#endregion

	#region navigation
	public void NextCell () {
		++col;
	}
	public void UpdateStory () {

		string content = GetContent;

		if ( content == null) {
			Debug.LogError ( " no function at index : " + col.ToString () + " / decal : " + row.ToString () );
		}

		StoryFunctions.Instance.Read ( content );

	}
	#endregion

	#region node & switch
	public void Node () {

		string text = StoryFunctions.Instance.CellParams;
		string nodeName = text.Remove (0, 2);
		Node node = GetNodeFromText (nodeName);
		GoToNode (node);

	}

	public void GoToNode (Node node) {

		StoryReader.Instance.Row = node.row;
		StoryReader.Instance.Col = node.col;

//		StoryReader.Instance.NextCell ();

		StoryReader.Instance.UpdateStory ();

	}

	public void Switch () {

		string text = StoryFunctions.Instance.CellParams;

		int decal = 1;

		string nodeName = text.Remove (0, 2);

		Node node = GetNodeFromText (nodeName);

		CurrentStoryHandler.SaveDecal (decal,node.row,node.col);

		StoryReader.Instance.NextCell ();
		StoryReader.Instance.UpdateStory ();


	}

	public Node GetNodeFromText ( string text ) {
		return GetNodeFromText (CurrentStoryHandler.Story, text);
	}

	public Node GetNodeFromText ( Story story, string text ) {

		text = text.TrimEnd ('\r', '\n', '\t');

		Node node = story.nodes.Find ( x => x.name == text);

		if ( node == null ) {
			Debug.LogError ("couldn't find node " + text + " // story : " + story.name);
            
			return null;
		}
		return node;
	}
	#endregion

	#region decal
	public void SetDecal ( int steps ) {

		while (steps > 0) {

			++row;
			string content = GetContent;

			if (content.Length > 0) {
				--steps;
			}


		}

	}

	public string ReadDecal (int decal) {
		return CurrentStoryHandler.Story.content
			[decal]
			[StoryReader.Instance.Col]; 

	}
	#endregion

	#region story layers
	public void ChangeStory () {

			// extract story informations
		string text = StoryFunctions.Instance.CellParams;

			// get story
		string storyName = text.Remove (0, 2);
		storyName = storyName.Remove (storyName.IndexOf ('['));

		// get second story
		Story secondStory = StoryLoader.Instance.FindByName (storyName,StoryType.Normal);

		// extract nodes
		string nodes = text.Remove (0,text.IndexOf ('[')+1);

		// get fallback nodes
		string fallbackNodeTXT = nodes.Split ('/') [1].TrimEnd(']');
		Node fallBackNode = GetNodeFromText (fallbackNodeTXT);

			// get target node
		string targetNodeTXT = nodes.Split ('/') [0];
		Node targetNode = GetNodeFromText (secondStory,targetNodeTXT);

		SetNewStory (secondStory, StoryType.Normal, targetNode, fallBackNode);
	}

	public void SetNewStory (Story story, StoryType storyType , Node targetNode , Node fallbackNode) {

		// rechercher l'id de l'histoire désirée
		int secondStoryID = StoryLoader.Instance.FindIndexByName (story.name,storyType);

		// rechercher si l'ile comprend déjà l'histoire désirée
		int targetStoryLayer = CurrentStoryManager.storyHandlers.FindIndex (handler => (handler.storyID == secondStoryID) );
		if ( targetStoryLayer >= 0 ) {
			// rechercher si l'histoire trouvée est bien à la rangée et collonne de la cellule actuelle.
			targetStoryLayer = CurrentStoryManager.storyHandlers.FindIndex (handler => (handler.row == row) && (handler.col == col));

		}

		// si l'histoire n'apparait pas déjà dans l'ile
		if ( targetStoryLayer < 0 ) {
			
			StoryHandler newHandler = new StoryHandler ( secondStoryID,storyType);
			newHandler.fallBackLayer = currentStoryLayer;
			newHandler.row = row;
			newHandler.col = col;
			newHandler.fallbackNode = fallbackNode;

			CurrentStoryManager.AddStoryHandler (newHandler);

			targetStoryLayer = CurrentStoryManager.storyHandlers.Count - 1;
		}

		previousStoryLayer = currentStoryLayer;
		currentStoryLayer = targetStoryLayer;
		
//		print("going to node s: " + targetNode.name + " row : " + targetNode.row + " col : " + targetNode.col);
		GoToNode (targetNode);
	}

	public void FallBackToPreviousStory () {

		Node fallbackNode = CurrentStoryHandler.fallbackNode;

		currentStoryLayer = CurrentStoryHandler.fallBackLayer;
		StoryReader.Instance.GoToNode (fallbackNode);
	}
	#endregion

	public void Wait ( float duration ) {
		waitToNextCell = true;
		timer = duration;
	}

	private void WaitForNextCell_Update () {

		timer -= Time.deltaTime;

		if (timer <= 0) {
			waitToNextCell = false;
			StoryReader.Instance.UpdateStory ();
		}
	}

	#region properties
	public int Col {
		get {
			return col;
		}
		set {
			col = value;
		}
	}

	public int Row {
		get {
			return row;
		}
		set {
			row = value;
		}
	}

	public string GetContent {
		get {
			
			if ( Row >= CurrentStoryHandler.Story.content.Count ) {

				Debug.LogError ("ROW is outside of story << " + CurrentStoryHandler.Story.name + " >> content : ROW : " + Row + " /// STORY CONTENT : " + CurrentStoryHandler.Story.content.Count);

				return "AAAAH";

				return CurrentStoryHandler.Story.content
					[0]
					[0];

			}

			if ( Col >= CurrentStoryHandler.Story.content [Row].Count ) {

				Debug.LogError ("INDEX is outside of story content : INDEX : " + Col + " /// COUNT : " + CurrentStoryHandler.Story.content[Row].Count);

				return "AAAAH";

				return CurrentStoryHandler.Story.content
					[Row]
					[0]; 
			}

			return CurrentStoryHandler.Story.content
				[Row]
				[Col];
		}
	}
	#endregion

	public StoryManager CurrentStoryManager {
		get {
			return currentStoryManager;
		}
		set {
			currentStoryManager = value;
		}
	}

	public StoryHandler CurrentStoryHandler {
		get {
			return CurrentStoryManager.storyHandlers[currentStoryLayer];
		}
		set {
			CurrentStoryManager.storyHandlers[currentStoryLayer] = value;
		}
	}
}
