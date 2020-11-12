 using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;

public class Island : RandomPlacable {

    public GameObject[] islandMeshes;

    public int id = 0;

	private Image image;

	[SerializeField]
	private float decal = 0f;

	[SerializeField]
	private GameObject group;

	[SerializeField]
	private Collider _collider = null;

    private Vector2 scale = Vector2.zero;

    public RectTransform uiBackground;

	[SerializeField]
	private RectTransform gameViewCenter;

    private IslandTrigger[] islandTriggers;

    public IslandData islandData;

    public bool targeted = false;

    #region mono
    public override void Start()
    {
        base.Start();

        image = GetComponentInChildren<Image>();

        islandTriggers = GetComponentsInChildren<IslandTrigger>(true);

        Init();
    }

    void Init () {

		WorldTouch.onPointerExit += HandleOnTouchWorld;

		UpdatePositionOnScreen (Boats.Instance.playerBoatInfo.coords);
	}

	void HandleOnTouchWorld ()
	{
		targeted = false;
        DeactivateCollider();
	}

    public override void HandleOnUpdateCurrentChunk()
    {
        base.HandleOnUpdateCurrentChunk();

        UpdatePositionOnScreen (Boats.Instance.playerBoatInfo.coords);
	}

	void DeactivateCollider ()
	{
        foreach (var item in islandTriggers)
        {
            item.DeactivateCollider();
        }
	}

	void ActivateCollider () {
        foreach (var item in islandTriggers)
        {
            item.ActivateCollider();
        }
	}
	#endregion

	#region story
	public void Enter () {

        Boats.Instance.WithdrawBoats();

        SoundManager.Instance.PlaySound("enter port");

        IslandManager.Instance.currentIsland = this;
        StoryLauncher.Instance.PlayStory(Chunk.currentChunk.GetIslandData(id).storyManager, StoryLauncher.StorySource.island);
	}
    #endregion

    #region render
    void UpdatePositionOnScreen(Coords coords) {

		Chunk chunk = Chunk.GetChunk (coords);

		if (chunk.HasIslands() && chunk.islandDatas.Length > id) {

            islandData = chunk.GetIslandData(id);

            gameObject.SetActive ( true );

            //GetComponent<RectTransform> ().anchoredPosition = chunk.IslandData.positionOnScreen;
            transform.localPosition = new Vector3(islandData.worldPosition.x  , 0f , islandData.worldPosition.y);

            //Debug.Log( "local position : " + transform.localPosition.x + " / local position " + transform.localPosition.y );

            transform.rotation = Quaternion.EulerAngles(0, islandData.worldRotation,0);

            //GetComponentInChildren<Image>().sprite = sprites [islandData.storyManager.storyHandlers [0].Story.param];

            foreach (var item in islandMeshes)
            {
                item.SetActive(false);
            }

            islandMeshes[islandData.storyManager.storyHandlers[0].Story.param].SetActive(true);

		} else {
			
			gameObject.SetActive ( false );

			transform.localPosition = new Vector3 (10000f, 0, 0);
		}
	}
    #endregion

    public override void OnMouseDown()
    {
        base.OnMouseDown();

        if (!WorldTouch.Instance.IsEnabled())
        {
            return;
        }

        ActivateCollider();

        SoundManager.Instance.PlayRandomSound("button_heavy");

        targeted = true;
    }

    public void CollideWithPlayer()
    {
        if (targeted)
        {
            Enter();
            targeted = false;
        }
    }
}
 