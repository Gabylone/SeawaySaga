//using UnityEngine;
//using UnityEngine.UI;
//using System.Collections;
//
//public class SaveMenu : MonoBehaviour {
////
////	[Header("Buttons")]
////	[SerializeField] private Button[] saveButtons;
////
////	[SerializeField]
////	private GameObject saveGroup;
////	[SerializeField]
////	private GameObject saveFeedback;
//
//	int currentSocket = -1;
//
//		// saving
//	private bool saving = false;
//
//	bool opened = false;
//
//	void Start()
//	{
//		Opened = false;
//	}
//
//	#region save action
//
////	public void SaveSocket (int index) {
////		Save (1);
////	}
//
//	public void SaveSocket (int index) {
////
////		Tween.Bounce ( saveButtons[index-1].transform , 0.2f , 1.1f );
////
////		if (saving) {
////
////			if (save_Confirmed) {
////				
////				Save (index);
////
////			} else {
////
////				if ( SaveTool.Instance.FileExists () ) {
////
////					saveFeedback.SetActive (true);
////					Tween.Bounce ( saveFeedback.transform , 0.2f , 1.1f );
////					saveFeedback.GetComponentInChildren<Text> ().text = "Ecraser ?";
////					save_Confirmed = true;
////
////				} else {
////
////					Save (index);
////
////				}
////			}
////
////		} else {
////
////			SaveManager.Instance.LoadGameCoroutine ();
////
////		}
//
//	}
//
//	private void Save (int index) {
//		
////		SaveManager.Instance.SaveGame ();
////		save_Confirmed = false;
////		saveFeedback.SetActive (false);
////
////		UpdateButtons ();
//
//	}
//	#endregion
//
//
//
//	#region buttons
//	public void UpdateButtons ()
//	{
//		int buttonIndex = 0;
//		int loadIndex = 1;
//
//		foreach (Button button in saveButtons) {
//
//			if (SaveTool.Instance.FileExists ()) {
//
//				GameData tempData =SaveTool.Instance.Load ();
//
//				saveButtons [buttonIndex].GetComponentInChildren<Text> ().text = tempData.playerCrew.MemberIDs [0].Name;
//				saveButtons [buttonIndex].GetComponentInChildren<Text> ().color = Color.black;
//				saveButtons[buttonIndex].image.color = Color.white;
//				saveButtons[buttonIndex].interactable = true;
//
//				saveButtons [buttonIndex].GetComponentInChildren<IconVisual> ().UpdateVisual (tempData.playerCrew.MemberIDs [0]);
//				foreach (SpriteRenderer rend in saveButtons [buttonIndex].GetComponentInChildren<IconVisual> ().GetComponentsInChildren<SpriteRenderer>())
//					rend.color = Color.white;
//
//			} else {
//
//				saveButtons [buttonIndex].GetComponentInChildren<Text> ().text = saving ? "?" : "aucun";
//				saveButtons [buttonIndex].GetComponentInChildren<Text> ().color = Color.white;
//				saveButtons[buttonIndex].image.color = Color.black;
//				saveButtons[buttonIndex].interactable = saving;
//
//				foreach (SpriteRenderer rend in saveButtons [buttonIndex].GetComponentInChildren<IconVisual> ().GetComponentsInChildren<SpriteRenderer>())
//					rend.color = Color.black;
//
//			}
//
//			++buttonIndex;
//			++loadIndex;
//		}
//	}
//
//
//	public bool Opened {
//		get {
//			return opened;
//		}
//		set {
//			opened = value;
//
////			saveGroup.SetActive (value);
//
////			if ( value == true ) {
////				Tween.Bounce ( saveGroup.transform , 0.2f , 1.1f );
////			}
////
//			save_Confirmed = false;
//
////			saveFeedback.SetActive (false);
//
//			if ( value == true )
//				UpdateButtons ();
//		}
//	}
//
//	public bool Saving {
//		get {
//			return saving;
//		}
//		set {
//			saving = value;
//		}
//	}
//	#endregion
//
//	public int CurrentSocket {
//		get {
//			return currentSocket;
//		}
//		set {
//			currentSocket = value;
//		}
//	}
//}
