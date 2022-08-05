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

    public static Island currentSelectedIsland;

    public GameObject[] selectFeedbacks;

    public GameObject currentSelectFeedback;

    #region mono
    public override void Start()
    {
        base.Start();


        image = GetComponentInChildren<Image>();

        islandTriggers = GetComponentsInChildren<IslandTrigger>(true);

        Deselect();

        WorldTouch.Instance.onSelectSomething += Deselect;

        Init();
    }

    void Init () {

		WorldTouch.onPointerDown += Deselect;

		UpdatePositionOnScreen (Boats.Instance.playerBoatInfo.coords);
	}

	public void Exit ()
	{
        Deselect();
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
    public void Enter()
    {

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
            _transform.localPosition = new Vector3(islandData.worldPosition.x, 0f, islandData.worldPosition.y);;
            //Debug.Log( "local position : " + _transform.localPosition.x + " / local position " + _transform.localPosition.y );

            _transform.rotation = Quaternion.EulerAngles(0, islandData.worldRotation,0);

            foreach (var item in islandMeshes)
            {
                item.SetActive(false);
            }

            int islandIndex = islandData.storyManager.storyHandlers[0].Story.param;
            islandMeshes[islandIndex].SetActive(true);
            currentSelectFeedback = selectFeedbacks[islandIndex];

		} else {
			
			gameObject.SetActive ( false );

            _transform.localPosition = new Vector3 (10000f, 0, 0);
		}
	}
    #endregion

    public override void OnMouseDown()
    {
        base.OnMouseDown();

        if (!WorldTouch.Instance.IsEnabled())
        {
            Debug.Log("world touch disable");
            return;
        }

        if (targeted)
        {
            return;
        }

        Debug.Log("mouse down");


        Select();
    }

    public void Select()
    {
        // deselect everything
        WorldTouch.Instance.onSelectSomething();

        if (currentSelectedIsland != null)
        {
            currentSelectedIsland.Deselect();
        }

        currentSelectedIsland = this;

        ActivateCollider();

        SoundManager.Instance.PlayRandomSound("button_heavy");

        targeted = true;

        if (currentSelectFeedback != null)
        {
            currentSelectFeedback.SetActive(true);
        }


    }

    public void Deselect()
    {
        targeted = false;
        DeactivateCollider();

        foreach (var item in selectFeedbacks)
        {
            item.SetActive(false);
        }
    }

    public void CollideWithPlayer()
    {
        if (targeted)
        {
            Enter();
            Deselect();
        }
    }
}
 