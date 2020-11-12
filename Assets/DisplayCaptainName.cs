using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DisplayCaptainName : MonoBehaviour, IPointerClickHandler
{
    public InputField InputField;

    private RectTransform rectTransform;

    // Start is called before the first frame update
    void Start()
    {
        Crews.playerCrew.captain.MemberID.Name = "The Captain";
        InputField.text = Crews.playerCrew.captain.MemberID.Name;

        InputField.onEndEdit.AddListener(delegate { ChangeName(InputField); });
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Tween.Bounce(rectTransform, 0.2f, 1.05f);

        SoundManager.Instance.PlaySound("click_med 03");

        InputField.interactable = true;
        InputField.ActivateInputField();
    }

    public void ChangeName(InputField inputField)
    {
        Tween.Bounce(rectTransform, 0.2f, 1.05f);
        Crews.playerCrew.captain.MemberID.Name = InputField.text;

        SoundManager.Instance.PlaySound("click_med 03");
    }
}
