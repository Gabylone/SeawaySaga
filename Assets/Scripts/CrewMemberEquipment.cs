using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrewMemberEquipment : MonoBehaviour {

	public GameObject[] buttonObjs;
	public float timeBetweenDisplay = 0.15f;
	public RectTransform rectTransform;

	public GameObject group;

	// Use this for initialization
	void Start () {

//		rectTransform = GetComponent<RectTransform> ();

		HideAll ();

		LootUI.onShowLoot += HandleOnShowLoot;
		LootUI.onHideLoot += HandleOnHideLoot;
	}

	void HandleOnHideLoot ()
	{
		CrewMember.GetSelectedMember.Icon.MoveToPoint (Crews.PlacingType.Inventory);
		StartCoroutine (HandleOnHideLootCoroutine ());
	}

	void HideAll ()
	{

		group.SetActive (false);

		foreach (var item in buttonObjs) {
			item.SetActive (false);
		}
	}

	void HandleOnShowLoot ()
	{
		//CrewMember.GetSelectedMember.Icon.MoveToPoint (Crews.PlacingType.Combat);
		StartCoroutine (HandleOnShowLootCoroutine ());
	}

	IEnumerator HandleOnShowLootCoroutine ()
	{
		group.SetActive (true);

		foreach (var item in buttonObjs) {
			
			item.SetActive (true);
			Tween.ClearFade(item.transform);
			Tween.Bounce (item.transform, timeBetweenDisplay, 1.1f);
			LayoutRebuilder.ForceRebuildLayoutImmediate (rectTransform);
			yield return new WaitForSeconds ( timeBetweenDisplay );

		}
//		LayoutRebuilder.ForceRebuildLayoutImmediate (rectTransform);
		yield return new WaitForEndOfFrame ();
	}

	IEnumerator HandleOnHideLootCoroutine ()
	{
		for (int i = 0; i < buttonObjs.Length; i++) {

			int index = buttonObjs.Length -1 - i;

			//			Tween.Bounce (buttonObjs [index].transform, timeBetweenButtonDisplay , 0.f);
			Tween.Fade (buttonObjs[index].transform, timeBetweenDisplay);

			yield return new WaitForSeconds ( timeBetweenDisplay );
//
//			if (index > 0) {
//				buttonObjs[index-1].SetActive(false);
//				LayoutRebuilder.ForceRebuildLayoutImmediate (rectTransform);
//				Tween.ClearFade(buttonObjs[index].transform);
//			}

		}

		HideAll ();
		yield return new WaitForEndOfFrame ();
	}
}
