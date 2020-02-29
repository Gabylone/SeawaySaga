using UnityEngine;
using System.Collections;

public class RandomManager : MonoBehaviour {

	public static RandomManager Instance;

	public int targetDecal = 0;

	void Awake( ) {
		Instance = this;
	}

	void Start () {
		StoryFunctions.Instance.getFunction += HandleGetFunction;
	}

	void HandleGetFunction (FunctionType func, string cellParameters)
	{
		switch (func) {
		case FunctionType.RandomRange:
			RandomRange (cellParameters);
			break;
		case FunctionType.RandomRedoRange:
			RandomRedoRange (cellParameters);
			break;
		case FunctionType.RandomPercent:
			RandomPercent (cellParameters);
			break;
		case FunctionType.RandomRedoPercent:
			RandomRedoPercent (cellParameters);
			break;
		}
	}

	#region random
	void RandomPercent (string cellParams) {

		float chance = float.Parse ( cellParams );

		float value = Random.value * 100;

		int randomDecal = value < chance ? 0 : 1;


		int decal = 0;

		if (StoryReader.Instance.CurrentStoryHandler.GetDecal() > -1) {
			decal = StoryReader.Instance.CurrentStoryHandler.GetDecal();
		} else {
			decal = randomDecal;
		}

		StoryReader.Instance.CurrentStoryHandler.SaveDecal (decal);

		StoryReader.Instance.NextCell ();
		StoryReader.Instance.SetDecal (decal);

		StoryReader.Instance.UpdateStory ();

	}

	void RandomRange (string cellParams) {

		int range = int.Parse (cellParams);
		int randomDecal = Random.Range (0, range);

		StoryReader.Instance.CurrentStoryHandler.SaveDecal (randomDecal);

		StoryReader.Instance.NextCell ();
		StoryReader.Instance.SetDecal (randomDecal);

		StoryReader.Instance.UpdateStory ();
	}

	void RandomRedoPercent (string cellParams) {

		float chance = float.Parse ( cellParams );

		float value = Random.value * 100;

		int decal = value < chance ? 0 : 1;

		StoryReader.Instance.NextCell ();
		StoryReader.Instance.SetDecal (decal);

		StoryReader.Instance.UpdateStory ();

	}
	void RandomRedoRange (string cellParams) {

		int range = int.Parse (cellParams);
		int randomDecal = Random.Range (0, range);

		StoryReader.Instance.NextCell ();
		StoryReader.Instance.SetDecal (randomDecal);

		StoryReader.Instance.UpdateStory ();

	}
	#endregion

}
