using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayStoryItem : MonoBehaviour {

    public static DisplayStoryItem Instance;

	[SerializeField]
	DisplayItem_Selected displayItem;

	[SerializeField]
	private GameObject group;

    private void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start () {
		StoryInput.onPressInput += HandleOnPressInput;

        group.SetActive(false); 
	}

	void HandleOnPressInput ()
	{
		group.SetActive (false);
	}

	public void DisplayItem (Item item)
	{
		group.SetActive (true);

		displayItem.HandledItem = item;

		Tween.Bounce (displayItem.transform);
	}
}
